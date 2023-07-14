namespace LoggerTool;

public class Logger
{
    public static async void SaveLog(string info)
    {
        try
        {
            using (StreamWriter file = new(AppContext.BaseDirectory + "/Console Music Player Logs.txt", true))
            {
                await file.WriteLineAsync(DateTime.Now + " " + info);
                file.Close();
            }
        }
        catch (Exception) { }
    }
}