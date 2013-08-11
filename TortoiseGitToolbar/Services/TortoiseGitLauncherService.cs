﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using MattDavies.TortoiseGitToolbar.Config.Constants;
using Process = System.Diagnostics.Process;

namespace MattDavies.TortoiseGitToolbar.Services
{
    public class TortoiseGitLauncherService
    {
        private readonly Solution _solution;
        private readonly string _tortoiseGitPath;
        private string _gitBashPath;

        public TortoiseGitLauncherService(Solution solution)
        {
            _solution = solution;

            if (File.Exists(TortoiseGitConstants.TortoiseGitx64))
                _tortoiseGitPath = TortoiseGitConstants.TortoiseGitx64;
            else if (File.Exists(TortoiseGitConstants.TortoiseGitx86))
                _tortoiseGitPath = TortoiseGitConstants.TortoiseGitx86;
            
            if (File.Exists(TortoiseGitConstants.GitBashx64))
                _gitBashPath = TortoiseGitConstants.GitBashx64;
            else if (File.Exists(TortoiseGitConstants.GitBashx86))
                _gitBashPath = TortoiseGitConstants.GitBashx86;
        }
        
        public void ExecuteTortoiseProc(string command)
        {
            var solutionPath = GetSolutionPath();

            //Todo: hide the buttons if not in an active solution (+ potentially in a git solution if we can detect that)
            if (solutionPath == null)
            {
                MessageBox.Show(
                    Resources.TortoiseGitLauncherService_SolutionPath_Please_open_a_solution_first,
                    Resources.TortoiseGitLauncherService_SolutionPath_No_solution_found,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
                return;
            }

            if (command == "bash")
            {
                LaunchProcess(_gitBashPath, "--login -i");
            }
            else
            {
                LaunchProcess(_tortoiseGitPath, string.Format("/command:{0} /path:{1}", command, solutionPath));
            }
        }

        private void LaunchProcess(string fileName, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments
            };
            var p = Process.Start(startInfo);
            p.WaitForInputIdle();
            MoveWindow(p.MainWindowHandle, 0, 0, 0, 0, false);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

        private string GetSolutionPath()
        {
            return _solution.IsOpen
                ? @"""" + Path.GetDirectoryName(_solution.FullName) + @""""
                : null;
        }
    }
}
