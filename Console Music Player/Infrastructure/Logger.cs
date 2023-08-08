namespace LoggerTool;

class Logger
{
    public static async void SaveLog(string info)
    {
        try
        {
            using (StreamWriter loggerFile = new(AppContext.BaseDirectory + "/Console Music Player Logs.txt", true))
            {
                await loggerFile.WriteLineAsync(DateTime.Now + " " + info);
                loggerFile.Close();
            }
        }
        catch (Exception) { }
    }
}