using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Windows_Personal_Assistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controller controller;
        private Task[] tasks;
        private Event[] events;
        private Note[] notes;
        private int selectedIndexHelper;

        public MainWindow()
        {
            InitializeComponent();
            controller = new Controller();
            InitialSetupAsync();
        }
        
        private async void InitialSetupAsync()
        {
            string tasksString = await controller.Get_TasksAsync();
            string notesString = await controller.Get_NotesAsync();
            string eventsString = await controller.Get_EventsAsync();
            tasks = JsonConvert.DeserializeObject<Task[]>(tasksString);
            events = JsonConvert.DeserializeObject<Event[]>(eventsString);
            notes = JsonConvert.DeserializeObject<Note[]>(notesString);
            HomeGridEventsListBox.Items.Clear();
            EventsGridEventsListBox.Items.Clear();
            HomeGridTasksListBox.Items.Clear();
            TasksGridTasksListBox.Items.Clear();
            NotesGridNotesListBox.Items.Clear();
            for (int i = 0; i < events.Length; i++)
            {
                HomeGridEventsListBox.Items.Add(events[i].title + " @ " + events[i].date.ToShortDateString());
                EventsGridEventsListBox.Items.Add(events[i].title + " @ " + events[i].date.ToShortDateString());
            }
            for (int i = 0; i < tasks.Length; i++)
            {
                HomeGridTasksListBox.Items.Add(tasks[i].title + ": " + tasks[i].description + " @ " + tasks[i].due_date.ToShortDateString());
                TasksGridTasksListBox.Items.Add(tasks[i].title + ": " + tasks[i].due_date.ToShortDateString());
            }
            for (int i = 0; i < notes.Length; i++)
            {
                NotesGridNotesListBox.Items.Add(notes[i].title);
            }


        }

        /*
         * Left Navigation Actions
         * 
         */
        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            EventsGrid.Visibility = Visibility.Hidden;
            TasksGrid.Visibility = Visibility.Hidden;
            NotesGrid.Visibility = Visibility.Hidden;
            HomeGrid.Visibility = Visibility.Visible;
        }

        private void Events_Button_Click(object sender, RoutedEventArgs e)
        {
            EventsGrid.Visibility = Visibility.Visible;
            TasksGrid.Visibility = Visibility.Hidden;
            NotesGrid.Visibility = Visibility.Hidden;
            HomeGrid.Visibility = Visibility.Hidden;
        }

        private void Tasks_Button_Click(object sender, RoutedEventArgs e)
        {
            EventsGrid.Visibility = Visibility.Hidden;
            TasksGrid.Visibility = Visibility.Visible;
            NotesGrid.Visibility = Visibility.Hidden;
            HomeGrid.Visibility = Visibility.Hidden;
        }

        private void Notes_Button_Click(object sender, RoutedEventArgs e)
        {
            EventsGrid.Visibility = Visibility.Hidden;
            TasksGrid.Visibility = Visibility.Hidden;
            NotesGrid.Visibility = Visibility.Visible;
            HomeGrid.Visibility = Visibility.Hidden;
        }

        /*
         * EventsGrid Actions
         * 
         */
        private void Create_Event_Button_Click(object sender, RoutedEventArgs e)
        {
            EventsGridEventsListBox.Visibility = Visibility.Hidden;
            CreateEventLabel.Visibility = Visibility.Visible;
            CreateEventTitleTextBox.Visibility = Visibility.Visible;
            CreateEventDatePicker.Visibility = Visibility.Visible;
            CreateEventSaveButton.Visibility = Visibility.Visible;
        }

        private async void Create_Event_Save_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            string title = CreateEventTitleTextBox.Text;
            DateTime? date = CreateEventDatePicker.SelectedDate;
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>()
                {
                    {"title", title},
                    {"date", date?.ToShortDateString()}
                };
            var result = await controller.Post_DataAsync(dataDictionary, "event", "create");
            Console.WriteLine(result);
            InitialSetupAsync();
            CreateEventTitleTextBox.Clear();
            CreateEventDatePicker.SelectedDate = DateTime.Now;
            EventsGridEventsListBox.Visibility = Visibility.Visible;
            CreateEventLabel.Visibility = Visibility.Hidden;
            CreateEventTitleTextBox.Visibility = Visibility.Hidden;
            CreateEventDatePicker.Visibility = Visibility.Hidden;
            CreateEventSaveButton.Visibility = Visibility.Hidden;
        }

        private void Edit_Event_Button_Click(object sender, RoutedEventArgs e)
        {
            selectedIndexHelper = EventsGridEventsListBox.SelectedIndex;
            if(selectedIndexHelper == -1)
            {
                MessageBox.Show("You must select a value to edit first");
            } else
            {
                CreateEventTitleTextBox.Text = events[selectedIndexHelper].title;
                CreateEventDatePicker.SelectedDate = events[selectedIndexHelper].date;
                EventsGridEventsListBox.Visibility = Visibility.Hidden;
                CreateEventLabel.Visibility = Visibility.Visible;
                CreateEventTitleTextBox.Visibility = Visibility.Visible;
                CreateEventDatePicker.Visibility = Visibility.Visible;
                CreateEventSaveButton.Visibility = Visibility.Visible;
                EditEventSaveButton.Visibility = Visibility.Visible;
            }
        }

        private async void Edit_Event_Save_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            string title = CreateEventTitleTextBox.Text;
            DateTime? date = CreateEventDatePicker.SelectedDate;
            string objectID = events[selectedIndexHelper]._id;
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>()
                {
                    {"id", objectID},
                    {"title", title},
                    {"date", date?.ToShortDateString()}
                };
            var result = await controller.Post_DataAsync(dataDictionary, "event", "edit");
            Console.WriteLine(result);

            InitialSetupAsync();
            CreateEventTitleTextBox.Clear();
            CreateEventDatePicker.SelectedDate = DateTime.Now;
            selectedIndexHelper = -1;
            EventsGridEventsListBox.Visibility = Visibility.Visible;
            CreateEventLabel.Visibility = Visibility.Hidden;
            CreateEventTitleTextBox.Visibility = Visibility.Hidden;
            CreateEventDatePicker.Visibility = Visibility.Hidden;
            CreateEventSaveButton.Visibility = Visibility.Hidden;
            EditEventSaveButton.Visibility = Visibility.Hidden;
        }

        private async void Delete_Event_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            selectedIndexHelper = EventsGridEventsListBox.SelectedIndex;
            if(selectedIndexHelper == -1)
            {
                MessageBox.Show("You must select a value first");
            } else
            {
                Dictionary<string, string> dataDictionary = new Dictionary<string, string>()
                {
                    {"id", events[selectedIndexHelper]._id}
                };
                var result = await controller.Post_DataAsync(dataDictionary, "event", "delete");
                InitialSetupAsync();
                selectedIndexHelper = -1;
            }
        }

        /*
         * TasksGrid Actions
         * 
         */
        private void TasksGridTasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedIndexHelper = TasksGridTasksListBox.SelectedIndex;
            if (selectedIndexHelper != -1)
            {
                TasksGridTaskDescriptionTextBox.Document.Blocks.Clear();
                TasksGridTaskDescriptionTextBox.Document.Blocks.Add(new Paragraph(new Run(tasks[selectedIndexHelper].description)));
            }
        }

        private void Create_Task_Button_Click(object sender, RoutedEventArgs e)
        {
            TasksTodoLabel.Visibility = Visibility.Hidden;
            TasksDescriptionLabel.Visibility = Visibility.Hidden;
            TasksGridTasksListBox.Visibility = Visibility.Hidden;
            TasksGridTaskDescriptionTextBox.Visibility = Visibility.Hidden;
            TasksEditTitleLabel.Visibility = Visibility.Visible;
            TasksEditTitleTextBox.Visibility = Visibility.Visible;
            TasksEditDescriptionLabel.Visibility = Visibility.Visible;
            TasksEditDescriptionTextBox.Visibility = Visibility.Visible;
            TasksEditDatePicker.Visibility = Visibility.Visible;
            TasksCreateSaveButton.Visibility = Visibility.Visible;
        }

        private async void Create_Task_Save_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>()
                {
                    {"title", TasksEditTitleTextBox.Text},
                    {"description", TasksEditDescriptionTextBox.Text},
                    {"due_date", TasksEditDatePicker.SelectedDate.Value.ToShortDateString()}
                };
            var result = await controller.Post_DataAsync(dataDictionary, "task", "create");
            Console.WriteLine(result);

            selectedIndexHelper = -1;
            TasksGridTasksListBox.SelectedIndex = -1;

            TasksGridTaskDescriptionTextBox.Document.Blocks.Clear();
            TasksTodoLabel.Visibility = Visibility.Visible;
            TasksDescriptionLabel.Visibility = Visibility.Visible;
            TasksGridTasksListBox.Visibility = Visibility.Visible;
            TasksGridTaskDescriptionTextBox.Visibility = Visibility.Visible;
            TasksEditTitleLabel.Visibility = Visibility.Hidden;
            TasksEditTitleTextBox.Visibility = Visibility.Hidden;
            TasksEditTitleTextBox.Clear();
            TasksEditDescriptionLabel.Visibility = Visibility.Hidden;
            TasksEditDescriptionTextBox.Visibility = Visibility.Hidden;
            TasksEditDescriptionTextBox.Clear();
            TasksEditDatePicker.Visibility = Visibility.Hidden;
            TasksEditDatePicker.SelectedDate = DateTime.Now;
            TasksCreateSaveButton.Visibility = Visibility.Hidden;
            InitialSetupAsync();
        }

        private void Edit_Task_Button_Click(object sender, RoutedEventArgs e)
        {
            selectedIndexHelper = TasksGridTasksListBox.SelectedIndex;
            if(selectedIndexHelper == -1)
            {
                MessageBox.Show("You must select a task to edit first");
            } else
            {
                TasksEditTitleTextBox.Text = tasks[selectedIndexHelper].title;
                TasksEditDescriptionTextBox.Text = tasks[selectedIndexHelper].description;
                TasksEditDatePicker.SelectedDate = tasks[selectedIndexHelper].due_date;

                TasksTodoLabel.Visibility = Visibility.Hidden;
                TasksDescriptionLabel.Visibility = Visibility.Hidden;
                TasksGridTasksListBox.Visibility = Visibility.Hidden;
                TasksGridTaskDescriptionTextBox.Visibility = Visibility.Hidden;
                TasksEditTitleLabel.Visibility = Visibility.Visible;
                TasksEditTitleTextBox.Visibility = Visibility.Visible;
                TasksEditDescriptionLabel.Visibility = Visibility.Visible;
                TasksEditDescriptionTextBox.Visibility = Visibility.Visible;
                TasksEditDatePicker.Visibility = Visibility.Visible;
                TasksEditSaveButton.Visibility = Visibility.Visible;
            }
        }

        private async void Edit_Task_Save_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>()
                {
                    {"id", tasks[selectedIndexHelper]._id},
                    {"title", TasksEditTitleTextBox.Text},
                    {"description", TasksEditDescriptionTextBox.Text},
                    {"due_date", TasksEditDatePicker.SelectedDate.Value.ToShortDateString()}
                };
            var result = await controller.Post_DataAsync(dataDictionary, "task", "edit");
            Console.WriteLine(result);

            selectedIndexHelper = -1;
            TasksGridTasksListBox.SelectedIndex = -1;

            TasksGridTaskDescriptionTextBox.Document.Blocks.Clear();
            TasksTodoLabel.Visibility = Visibility.Visible;
            TasksDescriptionLabel.Visibility = Visibility.Visible;
            TasksGridTasksListBox.Visibility = Visibility.Visible;
            TasksGridTaskDescriptionTextBox.Visibility = Visibility.Visible;
            TasksEditTitleLabel.Visibility = Visibility.Hidden;
            TasksEditTitleTextBox.Visibility = Visibility.Hidden;
            TasksEditTitleTextBox.Clear();
            TasksEditDescriptionLabel.Visibility = Visibility.Hidden;
            TasksEditDescriptionTextBox.Visibility = Visibility.Hidden;
            TasksEditDescriptionTextBox.Clear();
            TasksEditDatePicker.Visibility = Visibility.Hidden;
            TasksEditDatePicker.SelectedDate = DateTime.Now;
            TasksEditSaveButton.Visibility = Visibility.Hidden;
            InitialSetupAsync();
        }

        private async void Delete_Task_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            selectedIndexHelper = TasksGridTasksListBox.SelectedIndex;
            if(selectedIndexHelper == -1)
            {
                MessageBox.Show("You must select a task to delete first");
            } else
            {
                Dictionary<string, string> dataDictionary = new Dictionary<string, string>()
                {
                    {"id", tasks[selectedIndexHelper]._id}
                };
                var result = await controller.Post_DataAsync(dataDictionary, "task", "delete");
                Console.WriteLine(result);
                TasksGridTaskDescriptionTextBox.Document.Blocks.Clear();
                InitialSetupAsync();
            }
        }

        /*
         * NotesGrid Actions
         * 
         */
        private void NotesGridNotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedIndexHelper = NotesGridNotesListBox.SelectedIndex;
            if (selectedIndexHelper != -1)
            {
                NotesGridNoteBodyTextBox.Document.Blocks.Clear();
                NotesGridNoteBodyTextBox.Document.Blocks.Add(new Paragraph(new Run(notes[selectedIndexHelper].body)));
            }
        }

        private void Create_Note_Button_Click(object sender, RoutedEventArgs e)
        {
            NotesGridNotesListBox.Visibility = Visibility.Hidden;
            NotesTitlesLabel.Visibility = Visibility.Hidden;
            NotesBodyLabel.Visibility = Visibility.Hidden;
            NotesGridNoteBodyTextBox.Visibility = Visibility.Hidden;
            EditNoteTitleLabel.Visibility = Visibility.Visible;
            EditNoteBodyLabel.Visibility = Visibility.Visible;
            EditNoteTitleTextBox.Visibility = Visibility.Visible;
            EditNoteBodyTextBox.Visibility = Visibility.Visible;
            CreateNoteSaveNoteButton.Visibility = Visibility.Visible;
        }

        private async void Create_Note_Save_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>()
                {
                    {"title", EditNoteTitleTextBox.Text},
                    {"body", new TextRange(EditNoteBodyTextBox.Document.ContentStart, EditNoteBodyTextBox.Document.ContentEnd).Text}
                };
            var result = await controller.Post_DataAsync(dataDictionary, "note", "create");
            Console.WriteLine(result);

            selectedIndexHelper = -1;
            NotesGridNotesListBox.SelectedIndex = -1;
            NotesGridNoteBodyTextBox.Document.Blocks.Clear();
            NotesGridNotesListBox.Visibility = Visibility.Visible;
            NotesTitlesLabel.Visibility = Visibility.Visible;
            NotesBodyLabel.Visibility = Visibility.Visible;
            NotesGridNoteBodyTextBox.Visibility = Visibility.Visible;
            EditNoteTitleLabel.Visibility = Visibility.Hidden;
            EditNoteBodyLabel.Visibility = Visibility.Hidden;
            EditNoteTitleTextBox.Visibility = Visibility.Hidden;
            EditNoteBodyTextBox.Visibility = Visibility.Hidden;
            CreateNoteSaveNoteButton.Visibility = Visibility.Hidden;
            EditNoteTitleTextBox.Clear();
            EditNoteBodyTextBox.Document.Blocks.Clear();
            InitialSetupAsync();
        }

        private void Edit_Note_Button_Click(object sender, RoutedEventArgs e)
        {
            selectedIndexHelper = NotesGridNotesListBox.SelectedIndex;
            if(selectedIndexHelper == -1)
            {
                MessageBox.Show("You must select a note to edit first");
            } else
            {
                EditNoteBodyTextBox.Document.Blocks.Clear();
                NotesGridNotesListBox.Visibility = Visibility.Hidden;
                NotesTitlesLabel.Visibility = Visibility.Hidden;
                NotesBodyLabel.Visibility = Visibility.Hidden;
                NotesGridNoteBodyTextBox.Visibility = Visibility.Hidden;
                EditNoteTitleLabel.Visibility = Visibility.Visible;
                EditNoteBodyLabel.Visibility = Visibility.Visible;
                EditNoteTitleTextBox.Visibility = Visibility.Visible;
                EditNoteBodyTextBox.Visibility = Visibility.Visible;
                EditNoteSaveNoteButton.Visibility = Visibility.Visible;
                EditNoteTitleTextBox.Text = notes[selectedIndexHelper].title;
                EditNoteBodyTextBox.Document.Blocks.Add(new Paragraph(new Run(notes[selectedIndexHelper].body)));
            }
        }

        private async void Edit_Note_Save_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>()
                {
                    {"id", notes[selectedIndexHelper]._id},
                    {"title", EditNoteTitleTextBox.Text},
                    {"body", new TextRange(EditNoteBodyTextBox.Document.ContentStart, EditNoteBodyTextBox.Document.ContentEnd).Text}
                };
            var result = await controller.Post_DataAsync(dataDictionary, "note", "edit");
            Console.WriteLine(result);

            selectedIndexHelper = -1;
            NotesGridNotesListBox.SelectedIndex = -1;
            NotesGridNoteBodyTextBox.Document.Blocks.Clear();
            NotesGridNotesListBox.Visibility = Visibility.Visible;
            NotesTitlesLabel.Visibility = Visibility.Visible;
            NotesBodyLabel.Visibility = Visibility.Visible;
            NotesGridNoteBodyTextBox.Visibility = Visibility.Visible;
            EditNoteTitleLabel.Visibility = Visibility.Hidden;
            EditNoteBodyLabel.Visibility = Visibility.Hidden;
            EditNoteTitleTextBox.Visibility = Visibility.Hidden;
            EditNoteBodyTextBox.Visibility = Visibility.Hidden;
            EditNoteSaveNoteButton.Visibility = Visibility.Hidden;
            EditNoteTitleTextBox.Clear();
            EditNoteBodyTextBox.Document.Blocks.Clear();
            InitialSetupAsync();
        }

        private async void Delete_Note_Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            selectedIndexHelper = NotesGridNotesListBox.SelectedIndex;
            if (selectedIndexHelper == -1)
            {
                MessageBox.Show("You must select a note to delete first");
            }
            else
            {
                Dictionary<string, string> dataDictionary = new Dictionary<string, string>()
                {
                    {"id", notes[selectedIndexHelper]._id}
                };
                var result = await controller.Post_DataAsync(dataDictionary, "note", "delete");
                Console.WriteLine(result);
                NotesGridNoteBodyTextBox.Document.Blocks.Clear();
                InitialSetupAsync();
            }
        }

        
    }
}
