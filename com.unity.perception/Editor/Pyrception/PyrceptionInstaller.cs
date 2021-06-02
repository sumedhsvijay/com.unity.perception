using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Perception.GroundTruth;

public class PyrceptionInstaller : EditorWindow
{
    private static Process currentProcess = null;
    /// <summary>
    /// Runs pyrception instance in default browser
    /// </summary>
    [MenuItem("Window/Pyrception/Run")]
    static void RunPyrception()
    {
        /*if(currentProcess != null)
        {
            currentProcess.Kill();
            currentProcess = null;
            UnityEngine.Debug.Log("Current Process was set to null");
        }*/

        string path = Path.GetFullPath(Application.dataPath.Replace("/Assets", ""));
#if UNITY_EDITOR_WIN
        string packagesPath = Path.GetFullPath(Application.dataPath.Replace("/Assets","/Library/PythonInstall/Scripts"));
#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
        string packagesPath = Application.dataPath.Replace("/Assets","/Library/PythonInstall/bin");
#endif
        string pathToData = PlayerPrefs.GetString(SimulationState.latestOutputDirectoryKey);
#if UNITY_EDITOR_WIN
        path = path.Replace("/", "\\");
        packagesPath = packagesPath.Replace("/", "\\");
        pathToData = pathToData.Replace("/", "\\");
#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
        //path = path.Replace(" ", "\\ ");
        //packagesPath = packagesPath.Replace(" ", "\\ ");
        //pathToData = pathToData.Replace(" ", "\\ ");
#endif
        string command = "";

#if UNITY_EDITOR_WIN
        //command = $"cd \"{path}\\DataInsightsEnv\\Scripts\\\" && activate && cd \"{pathToData}\\..\" && \"{path}\\DataInsightsEnv\\Scripts\\pyrception-utils.exe\" preview --data=\".\"";
        command = $"cd \"{pathToData}\\..\" && \"{packagesPath}\\pyrception-utils.exe\" preview --data=\".\"";
#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
        //command = $"cd \"{path}/DataInsightsEnv/bin\";source activate;cd \"{pathToData}/..\";\"{path}/DataInsightsEnv/bin/pyrception-utils\" preview --data=\".\"";
        command = $"cd \'{packagesPath}\' ;./python3.7 ./pyrception-utils.py preview --data=\'{pathToData}/..\'";
#endif
        int ExitCode = 0;
        ExecuteCMD(command, ref ExitCode, waitForExit: true, displayWindow: true);
        if (ExitCode != 0)
            return;
        else
            UnityEngine.Debug.Log("You can view a preview of your datasets at: <color=#00aaccff>http://localhost:8501</color>");
    }

    /// <summary>
    /// Install pyrception (Assumes python3 and pip3 are already installed)
    /// - installs virtualenv if it is not already installed
    /// - and setups a virtual environment for pyrception
    /// </summary>
    [MenuItem("Window/Pyrception/Setup")]
    static void SetupPyrception()
    {
        int steps = 3;

        //==============================CHECK PIP3 IS INSTALLED======================================
        int ExitCode = 0;
        /*ExecuteCMD("pip3", ref ExitCode);
        if(ExitCode != 0)
        {
            UnityEngine.Debug.LogError("Python >= 3 and pip3 must be installed and available on PATH.");
            return;
        }*/

        //==============================SETUP PATHS======================================
#if UNITY_EDITOR_WIN
        string packagesPath = Path.GetFullPath(Application.dataPath.Replace("/Assets","/Library/PythonInstall/Scripts"));
#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
        string packagesPath = Path.GetFullPath(Application.dataPath.Replace("/Assets","/Library/PythonInstall/bin"));
#endif
        string pyrceptionPath = Path.GetFullPath("Packages/com.unity.perception/Editor/Pyrception/pyrception-utils").Replace("\\","/");

#if UNITY_EDITOR_WIN
        pyrceptionPath = pyrceptionPath.Replace("/", "\\");
        packagesPath = packagesPath.Replace("/", "\\");
#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
        //pyrceptionPath = pyrceptionPath.Replace(" ", "\\ ");
        //packagesPath = packagesPath.Replace(" ", "\\ ");
#endif


        //==============================INSTALL VIRTUALENV======================================
        //EditorUtility.DisplayProgressBar("Setting up Pyrception", "Installing virtualenv...", 0 / steps);

        ExitCode = 0;
#if UNITY_EDITOR_WIN
        //ExecuteCMD($"\"{packagesPath}\\pip3.bat\" install virtualenv", ref ExitCode);
#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
        //ExecuteCMD($"pip3 install --target=\"{path}/virtualenvDI\" virtualenv", ref ExitCode); //(maybe add --no-user)
#endif
        /*if (ExitCode != 0) {
            EditorUtility.ClearProgressBar();
            return;
        }*/

        //==============================CREATE VIRTUALENV NAMED DataInsightsEnv======================================
        //EditorUtility.DisplayProgressBar("Setting up Pyrception", "Setting up virtualenv instance...", 1f / steps);
#if UNITY_EDITOR_WIN
        //ExecuteCMD($"\"{packagesPath}\\virtualenv.exe\" -p python3 \"{path}\\DataInsightsEnv\"", ref ExitCode);
#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
        //string virtualenvPath = path+"/virtualenvDI/bin/";
        //ExecuteCMD("export PYTHONPATH=\"${PYTHONPATH}:"+$"{path}/virtualenvDI\";"+$"\"{virtualenvPath}/virtualenv\" -p python3 \"{path}/DataInsightsEnv\"", ref ExitCode);
#endif
        /*if (ExitCode != 0) {
            EditorUtility.ClearProgressBar();
            return;
        }*/

        //==============================COPY ALL PYRCEPTION FILES FOR INSTALLATION======================================
        EditorUtility.DisplayProgressBar("Setting up Pyrception", "Getting pyrception files...", 1.5f / steps);

#if UNITY_EDITOR_WIN
        //ExecuteCMD($"XCOPY /E/I/Y \"{pyrceptionPath}\" \"{path}\\DataInsightsEnv\\pyrception-util\"", ref ExitCode);
        //ExecuteCMD($"XCOPY /E/I/Y \"{pyrceptionPath}\" \"{packagesPath}\\..\\pyrception-util\"", ref ExitCode);
#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
        //ExecuteCMD($"\\cp -r \"{pyrceptionPath}\" \"{path}/DataInsightsEnv/pyrception-util\"", ref ExitCode);
        ExecuteCMD($"\\cp -r \'{pyrceptionPath}\' \'{packagesPath}/../pyrception-util\'", ref ExitCode);
#endif
        if (ExitCode != 0) {
            EditorUtility.ClearProgressBar();
            return;
        }

        /*UnityEngine.Debug.Log(pyrceptionPath);
        UnityEngine.Debug.Log(packagesPath);
        DirectoryCopy(pyrceptionPath, packagesPath);*/

        EditorUtility.DisplayProgressBar("Setting up Pyrception", "Installing pyrception utils...", 2.5f / steps);
        
        //==============================INSTALL PYRCEPTION IN THE VIRTUALENV======================================
#if UNITY_EDITOR_WIN
        //ExecuteCMD($"\"{path}\\DataInsightsEnv\\Scripts\\activate\" && cd \"{path}\\DataInsightsEnv\\pyrception-util\" && \"{packagesPath}\"\\pip3 --no-cache-dir install -e . && deactivate", ref ExitCode);
        ExecuteCMD($"cd \"{packagesPath}\\..\\pyrception-util\" && \"{packagesPath}\"\\pip3.bat install --no-warn-script-location --no-cache-dir -e .", ref ExitCode);

#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
        //ExecuteCMD($"source \"{path}/DataInsightsEnv/bin/activate\"; cd \"{path}/DataInsightsEnv/pyrception-util\"; pip3 --no-cache-dir install -e .; deactivate", ref ExitCode);
        ExecuteCMD($"cd \'{packagesPath}\'; ./python3.7 -m pip install -e \'../pyrception-util/.\'", ref ExitCode);
#endif
        if (ExitCode != 0) {
            EditorUtility.ClearProgressBar();
            return;
        }

        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// Executes command in cmd or console depending on system
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="waitForExit">Should it wait for exit before returning to the editor (i.e. is it not async?)</param>
    /// <param name="displayWindow">Should the command window be displayed</param>
    /// <returns></returns>
    private static string ExecuteCMD(string command, ref int ExitCode, bool waitForExit = true, bool displayWindow = false, bool redirectOutput = false)
    {
        string shell = "";
        string argument = "";
        string output = "";

        UnityEngine.Debug.Log(command);

#if UNITY_EDITOR_WIN
        shell = "cmd.exe";
        argument = $"/c \"{command}\"";
#elif (UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX)
        shell = "/bin/bash";
        argument = $"-c \"{command}\"";
#endif
        ProcessStartInfo info = new ProcessStartInfo(shell, argument);

        info.CreateNoWindow = !displayWindow;
        info.UseShellExecute = !waitForExit;
        info.RedirectStandardOutput = redirectOutput && waitForExit;
        info.RedirectStandardError = waitForExit;

        Process cmd = Process.Start(info);

        if (!waitForExit)
        {
            currentProcess = cmd;
            return "";
        }


        cmd.WaitForExit();
        if (redirectOutput) {
            output = cmd.StandardOutput.ReadToEnd();
        }

        ExitCode = cmd.ExitCode;
        if (ExitCode != 0)
        {
            UnityEngine.Debug.LogError($"Error - {ExitCode} - Failed to execute: {command} - {cmd.StandardError.ReadToEnd()}");
        }

        cmd.Close();

        return output;
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName, bool first = true)
    {
        bool copySubDirs = true;

        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();

        // If the destination directory doesn't exist, create it.
        DirectoryInfo dirDest = new DirectoryInfo(destDirName);
        if (dirDest.Exists && first)
        {
            DeleteDirectory(destDirName);
        }

        Directory.CreateDirectory(destDirName);

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
            UnityEngine.Debug.Log("Copying " + file.Name + " to " + tempPath);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, false);
            }
        }
    }

    private static void DeleteDirectory(string target_dir)
    {
        string[] files = Directory.GetFiles(target_dir);
        string[] dirs = Directory.GetDirectories(target_dir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(target_dir, false);
    }
}
