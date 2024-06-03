using Npgsql;

public class DataBaseRequests
{
     public static int UsernameQuery(string username, string password)
    {
        var querySql = $"SELECT user_id FROM users WHERE username = '{username}' AND password = '{password}'";

        using var cmdQuery = new NpgsqlCommand(querySql, DataBaseService.GetSqlConnection());
        var result = cmdQuery.ExecuteScalar();
        return Convert.ToInt32(result);
    }
     
     public static int UsernameCheckQuery(string login)
     {
         var querySql = $"SELECT user_id FROM users WHERE username = '{login}'";
         using var cmdQuerySql = new NpgsqlCommand(querySql, DataBaseService.GetSqlConnection());
         var resultCheckLogin = cmdQuerySql.ExecuteScalar();
         if (resultCheckLogin != null)
         {
             return Convert.ToInt32(resultCheckLogin);
         }
         Console.WriteLine();
         Console.WriteLine(" Неверный логин или пароль ");
         return 0;
     }

    public static int SignUpUserQuery(string username, string password)
    {
        var querySqlCheckUser =  $"SELECT user_id FROM users WHERE username = '{username}' and password = '{password}'";
        using var cmdQuerySqlCheckUser = new NpgsqlCommand(querySqlCheckUser, DataBaseService.GetSqlConnection());
        var resultCheckUser = cmdQuerySqlCheckUser.ExecuteScalar();
        if (resultCheckUser == null)
        {
            var querySqlSignUp = $"INSERT INTO users (username, password) VALUES  ('{username}', '{password}')";

            using var cmdQuerySqlSignUp = new NpgsqlCommand(querySqlSignUp, DataBaseService.GetSqlConnection());
            var resultRegistration = cmdQuerySqlSignUp.ExecuteScalar();
            
            var querySqlGetId = $"SELECT user_id FROM users " +
                                $"WHERE username = '{username}' AND password = '{password}'";

            using var cmdQuerySqlGetId = new NpgsqlCommand(querySqlGetId, DataBaseService.GetSqlConnection());
            var resultGetId = cmdQuerySqlGetId.ExecuteScalar();
            if (resultGetId != null)
            {
                return Convert.ToInt32(resultGetId);
            }
        }
        else
        {
            return -1;
        }

        return 0;
    }
    
    public static void AddTaskQuery(int userId, string name, string description, string time)
    {
        // Проверяем, существует ли пользователь с указанным userId
        var querySqlCheckUser = $"SELECT user_id FROM users WHERE user_id = {userId}";
        using (var cmdQuerySqlCheckUser = new NpgsqlCommand(querySqlCheckUser, DataBaseService.GetSqlConnection()))
        {
            var resultCheckUser = cmdQuerySqlCheckUser.ExecuteScalar();
            if (resultCheckUser == null)
            {
                Console.WriteLine($"Пользователь с ID {userId} не найден.");
                return;
            }
        }

        // Добавляем задачу
        var querySql = $"INSERT INTO tasks (user_id, name, description, time) VALUES ({userId}, '{name}', '{description}', '{time}')";
        using (var cmdQuery = new NpgsqlCommand(querySql, DataBaseService.GetSqlConnection()))
        {
            cmdQuery.ExecuteNonQuery();
            Console.WriteLine("Задача добавлена");
        }
    }

    public static void DeleteTaskQuery(int taskId)
    {
        var querySql = $"DELETE FROM tasks WHERE task_id = '{taskId}'";

        using var cmdQuery = new NpgsqlCommand(querySql, DataBaseService.GetSqlConnection());
        cmdQuery.ExecuteNonQuery();
        Console.WriteLine();
        Console.WriteLine("Задача удалена");
    }

    public static void EditTaskQuery(int taskid, string title, string description, string time)
    {
        var querySql = $"UPDATE tasks SET name = '{title}', description = '{description}', time = '{time}' WHERE task_id = '{taskid}'";

        using var cmdQuery = new NpgsqlCommand(querySql, DataBaseService.GetSqlConnection());
        cmdQuery.ExecuteNonQuery();
        Console.WriteLine();
        Console.WriteLine("Задача изменена");
    }
    
    public static List<Task> GetTasksForUserQuery(int userId)
    {
        List<Task> tasks = new List<Task>();

        var querySql = $"SELECT task_id, user_id, name, description, time FROM tasks WHERE user_id = '{userId}'";

        using var cmdQuery = new NpgsqlCommand(querySql, DataBaseService.GetSqlConnection());

        using (NpgsqlDataReader reader = cmdQuery.ExecuteReader())
        {
            while (reader.Read())
            {
                int taskIdRead = reader.GetInt32(0);
                int userIdRead = reader.GetInt32(1);
                string nameRead = reader.GetString(2);
                string descriptionRead = reader.GetString(3);
                DateTime timeRead = reader.GetDateTime(4);

                Task task = new Task(taskIdRead, userIdRead, nameRead, descriptionRead, timeRead);
                tasks.Add(task);
            }
        }

        return tasks;

    }
    
    public static Task GetTask(int taskId)
    {
        var querySql = $"SELECT name, description, time FROM tasks WHERE task_id = '{taskId}'";

        using var cmdQuery = new NpgsqlCommand(querySql, DataBaseService.GetSqlConnection());

        using (NpgsqlDataReader reader = cmdQuery.ExecuteReader())
        {
            if (reader.Read())
            {
                string nameRead = reader.GetString(0);
                string descriptionRead = reader.GetString(1);
                DateTime timeRead = reader.GetDateTime(2);

                Task task = new Task(nameRead, descriptionRead, timeRead);
            
                return task;
            }

            return null;
        }
    }
    
    public static int CheckTaskQuery(int userId, int taskId)
    {
        var querySqlCheckTask = $"SELECT name FROM tasks WHERE task_id = '{taskId}' and user_id = '{userId}'";

        using var cmdQuerySqlCheckTask = new NpgsqlCommand(querySqlCheckTask, DataBaseService.GetSqlConnection());
        var resultCheckTask = cmdQuerySqlCheckTask.ExecuteScalar();
        if (resultCheckTask == null)
        {
            return -1;
        }

        return 0;
    }
    
    public class Task
    {
        private int TaskId { get; set; }
        private int UserId { get; set; }
        private string Title { get; set; }
        private string Description { get; set; }
        private DateTime Time { get; set; }

        public Task(int taskId, int userId, string title, string description, DateTime time)
        {
            TaskId = taskId;
            UserId = userId;
            Title = title;
            Description = description;
            Time = time;
        }

        public Task(string title, string description, DateTime time)
        {
            Title = title;
            Description = description;
            Time = time;
        }

        public int GetTaskId()
        {
            return TaskId;
        }

        public int GetUserId()
        {
            return UserId;
        }

        public string GetTitle()
        {
            return Title;
        }

        public string GetDescription()
        {
            return Description;
        }

        public DateTime GetDueDate()
        {
            return Time;
        }
    }

    
    
}
 