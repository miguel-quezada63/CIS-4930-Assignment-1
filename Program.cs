// Miguel Quezada
// Assignment #1
// 09/09/2021

using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskApp
{
    class Task
    {
        public string Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsCompleted { get; set; }

        public Task(string id, string name, string description, DateTime deadline, bool isCompleted)
        {
            Id = id;
            Name = name;
            Description = description;
            Deadline = deadline;
            IsCompleted = isCompleted;
        }
        public void Log() => 
            Console.WriteLine("ID: {0}\nName: {1}\nDescription: {2}\nDeadline: {3}\nIsCompleted: {4}\n", Id, Name, Description, Deadline.Date.ToShortDateString(), IsCompleted);
    }

    class TaskManager
    {
        public List<Task> Tasks { get; } = new List<Task>();
        private string GenerateId()
        {
            var id = Guid.NewGuid().ToString("N").Substring(0, 6);
            while(Tasks.Find(task => task.Id == id) != default(Task))
                    id = Guid.NewGuid().ToString("N").Substring(0, 6);
            return id;
        }
        public void CreateTask(string name, string description, DateTime deadline, bool isCompleted) =>
            Tasks.Add(new Task(GenerateId(), name, description, deadline, isCompleted));
        public bool DeleteTask(string id) => Tasks.Remove(FindTask(id));
        public bool EditTask(string id, string newName, string newDesc, DateTime newDeadline)
        {
            if (Tasks.Count <= 0) return false; 
            Task t = FindTask(id);
            if (t == default(Task)) return false;
            if (newName != string.Empty) t.Name = newName;
            if (newDesc != string.Empty) t.Description = newDesc;
            if (newDeadline != DateTime.MinValue) t.Deadline = newDeadline;
            return true;
        }
        public bool CompleteTask(string id)
        {
            var t = FindTask(id);
            if (t == default(Task)) return false;
            t.IsCompleted = true;
            return true;
        }
        public void ListOutstandingTasks()
        {
            var outstandingTasks = Tasks.FindAll(task => !task.IsCompleted);
            if (outstandingTasks.Count > 0)
                outstandingTasks.ForEach(task => task.Log());
            else
                Console.WriteLine("There are no outstanding tasks to display.");
        }
        public void ListAllTasks()
        {
            if (Tasks.Count > 0) 
                Tasks.ForEach(task => task.Log());
            else
                Console.WriteLine("There are no tasks to display.");
        }
        private Task FindTask(string id) =>
            Tasks.Find(task => task.Id == id);
    }
    class Program
    {
        private static class MenuCommand
        {
            internal const string Create = "create";
            internal const string Delete = "delete";
            internal const string Edit = "edit";
            internal const string Complete = "complete";
            internal const string ListOutstanding = "listout";
            internal const string ListAll = "listall";
            internal const string Exit = "exit";
        }

        private static readonly TaskManager TaskManager = new TaskManager();

        static void Main(string[] args)
        {
            for(string input = ""; input != MenuCommand.Exit;)
            {
                PrintMenu();
                input = Console.ReadLine().ToLower();
                string inputTrimmed = String.Concat(input.Where(c => !Char.IsWhiteSpace(c)));
                Console.WriteLine();
                switch (inputTrimmed)
                {
                    case MenuCommand.Create:
                        Create();
                        break;
                    case MenuCommand.Edit:
                        Edit();
                        break;
                    case MenuCommand.Delete:
                        Delete();
                        break;
                    case MenuCommand.Complete:
                        Complete();
                        break;
                    case MenuCommand.ListOutstanding:
                        TaskManager.ListOutstandingTasks();
                        break;
                    case MenuCommand.ListAll:
                        TaskManager.ListAllTasks();
                        break;
                    case MenuCommand.Exit:
                        break;
                    default:
                        Console.Write("Invalid selection, please try again.\n");
                        break;
                }
                Console.WriteLine();
            }
        }
        private static void PrintMenu()
        {
            Console.Write(
                "Please select an option from the following menu:\n\n" +
                "Create a new task: {0}\n" +
                "Delete an existing task: {1}\n" +
                "Edit an existing task: {2}\n" +
                "Set an existing task to completed: {3}\n" +
                "List all outstanding tasks: {4}\n" +
                "List all tasks: {5}\n\n" +
                "Input: ",
                MenuCommand.Create,
                MenuCommand.Delete,
                MenuCommand.Edit,
                MenuCommand.Complete,
                MenuCommand.ListOutstanding,
                MenuCommand.ListAll);
        }
        private static void Create()
        {
            Console.Write("Please enter the name of the task: ");
            var name = Console.ReadLine();
            Console.Write("Please enter the description of the task: ");
            var description = Console.ReadLine();
            Console.Write("Please enter the deadline of the task (MM/DD/YYYY): ");
            DateTime deadline;
            while (!DateTime.TryParse(Console.ReadLine(), out deadline))
                Console.Write("Invalid date entered. Please enter the deadline of the task (MM/DD/YYYY): ");
            Console.Write("Is the task completed (y/N): ");
            char isCompleted = Char.ToLower(Char.Parse(Console.ReadLine()));
            TaskManager.CreateTask(name, description, deadline, isCompleted == 'y');
        }
        private static void Edit()
        {
            Console.Write("Please enter the ID of the task you would like to edit: ");
            if (TaskManager.Tasks.Count <= 0)
            {
                Console.Write("There are currently no tasks to edit, please create a task first before editing.");
                return;
            }
            string id = Console.ReadLine();
            Console.WriteLine("Please enter the new information for each respective attribute. If you do not want to edit an attribute, simply leave the input blank.");
            Console.Write("Please enter the new name of the task (leave empty if no change): ");
            string newName = Console.ReadLine();
            Console.Write("Please enter the new description of the task (leave empty if no change): ");
            string newDescription = Console.ReadLine();
            Console.WriteLine("Please enter the new deadline of the task (MM/DD/YYYY): ");
            DateTime newDeadline;
            // fix DateTime behavior that won't allow date to be empty here
            while (!DateTime.TryParse(Console.ReadLine(), out newDeadline))
                Console.Write("Invalid date entered. Please enter the new deadline of the task (MM/DD/YYYY): ");
            bool isSuccessfulEdit = TaskManager.EditTask(id, newName, newDescription, newDeadline);
            Console.WriteLine(
                isSuccessfulEdit ? "\nTask with ID {0} successfully edited." : "\nTask with ID {0} not found.", id);
        }
        private static void Delete()
        {
            Console.Write("Please enter the ID of the task you would like to delete: ");
            string id = Console.ReadLine();
            bool isSuccessfullyDeleted = TaskManager.DeleteTask(id);
            Console.WriteLine(
                isSuccessfullyDeleted ? "\nTask with ID {0} successfully deleted." : "\nTask with ID {0} not found.",
                id);
        }
        private static void Complete()
        {
            Console.Write("Please enter the ID of the task you would like to complete: ");
            string id = Console.ReadLine();
            bool isSuccessfullyCompleted = TaskManager.CompleteTask(id);
            Console.WriteLine(
                isSuccessfullyCompleted
                    ? "\nTask with ID {0} successfully set to complete."
                    : "\nTask with ID {0} unsuccessfully set to complete.", id);
        }
    }
}