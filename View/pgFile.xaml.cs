using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Cracker
{
  public partial class pgFile : Page
  {
    private Task cTask;

    public pgFile(Task task)
    {
      InitializeComponent();
      this.cTask = task;
    }

    private void Refresh(File file=null)
    {
      App.cDB.tasks.Load();

      File sel = file != null ? file : ((File)(grdFile.SelectedItem));
      grdFile.ItemsSource = App.cDB.files.
        Where<File>(p => p.task_id == cTask.id).ToList();
      if (sel != null)
      {
        grdFile.SelectedItem = sel;
        grdFile.ScrollIntoView(sel);
      }
      grdFile.Focus();
    } 

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
      ((MainWindow)(App.Current.MainWindow)).txtCapt.Text = Title+cTask;

      Refresh();
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
      File file = new File();
      file.task_id = cTask.id;
      file.name = "файл";
      App.cDB.files.Add(file)  ;
      try { App.cDB.SaveChanges(); }
      catch (Exception ex) { MessageBox.Show(ex.Message); }
      Refresh(file);
    }

    private void btnRemove_Click(object sender, RoutedEventArgs e)
    {
      if (grdFile.SelectedIndex == -1) return;

      if (MessageBox.Show("Вы уверены?", "Удалить",
              MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;

      App.cDB.files.Remove((File)grdFile.SelectedItem);
      try { App.cDB.SaveChanges(); }
      catch (Exception ex) { MessageBox.Show(ex.Message); }
      Refresh();
    }
    
    private void grdKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        e.Handled = true;
        grdFile.CommitEdit();
      }
    }

    private bool locker = false;
    private void grdCellEditEnd(object sender, DataGridCellEditEndingEventArgs e)
    {
      if (e.EditAction == DataGridEditAction.Cancel) return;
      if (locker) return;
      locker = true;
      ((DataGrid)sender).CommitEdit(DataGridEditingUnit.Row, false);
      locker = false;
      try { App.cDB.SaveChanges(); }
      catch (Exception ex) { MessageBox.Show(ex.Message); }
      Refresh();
    }
  }
}

