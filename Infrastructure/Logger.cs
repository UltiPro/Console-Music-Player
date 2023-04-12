namespace LoggerTool;

public class Logger
{
    public static async void SaveLog(string info)
    {
        using (StreamWriter file = new("AppLogs.txt", true))
        {
            await file.WriteLineAsync(DateTime.Now + " " + info);
            file.Close();
        }
    }
}