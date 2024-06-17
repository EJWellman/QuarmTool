﻿using EQTool.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EQTool.Services
{
	public class FindEq
	{
		private class Match : FindEQData
		{
			public DateTime LastModifiedDate { get; set; }
			public bool HasCharUiFiles { get; set; }
		}

		public class FindEQData
		{
			public string EqBaseLocation { get; set; }

			public string EQlogLocation { get; set; }
		}

		public FindEQData LoadEQPath()
		{
#if QUARM
			return this.LoadEQPath_ForQuarm();
#else
            return this.LoadEQPath_ForP99();
#endif
		}

		private FindEQData LoadEQPath_ForQuarm()
		{
			var possibles = new ConcurrentQueue<Match>();
			var drives = DriveInfo.GetDrives().Where(a => a.IsReady && a.DriveType == DriveType.Fixed);
			var tasks = new List<Task>();
			foreach (var f in drives)
			{
				tasks.Add(Task.Factory.StartNew(() =>
				{
					var files = GetFilesAndFolders(f.Name, "eqgame.exe", 2);
					foreach (var item in files)
					{
						try
						{
							var root = Path.GetDirectoryName(item);
							var eqmaxexists = false;
							try
							{
								var directory = new DirectoryInfo(root);
								eqmaxexists = directory.GetFiles("eqmac.exe", SearchOption.TopDirectoryOnly).Any();
							}
							catch { }
							if (eqmaxexists)
							{
								var dirdata = GetUIFiles(root)
									.OrderByDescending(a => a.LastWriteTime)
									.Select(a => new { a.LastWriteTime, a.FullName })
									.FirstOrDefault();
								if (dirdata != null)
								{
									var directory = new DirectoryInfo(root);
									var maxmoddate = directory.GetFiles()
									.OrderByDescending(a => a.LastWriteTime)
									.Select(a => (DateTime?)a.LastWriteTime)
									.FirstOrDefault();
									if (maxmoddate.HasValue)
									{
										possibles.Enqueue(new Match
										{
											LastModifiedDate = maxmoddate.Value,
											EqBaseLocation = root,
											HasCharUiFiles = false
										});
									}
								}
								else
								{
									possibles.Enqueue(new Match
									{
										LastModifiedDate = dirdata.LastWriteTime,
										EqBaseLocation = root,
										HasCharUiFiles = true
									});
								}
							}
						}
						catch { }
					}
				}));
			}
			var allran = Task.WaitAll(tasks.ToArray(), 1000 * 5);
			var rootFolder = possibles.Where(a => a.HasCharUiFiles).OrderByDescending(a => a.LastModifiedDate).FirstOrDefault();
			if (rootFolder == null)
			{
				rootFolder = possibles.OrderByDescending(a => a.LastModifiedDate).FirstOrDefault();
			}

			if (rootFolder != null)
			{
				var logFiles = GetLogFileLocation(new FindEQData
				{
					EqBaseLocation = rootFolder.EqBaseLocation,
					EQlogLocation = rootFolder.EQlogLocation,
				});
				if (logFiles.Found)
				{
					rootFolder.EQlogLocation = logFiles.Location;
				}
			}

			return rootFolder;
		}

		private FindEQData LoadEQPath_ForP99()
		{
			var possibles = new ConcurrentQueue<Match>();
			var drives = DriveInfo.GetDrives().Where(a => a.IsReady && a.DriveType == DriveType.Fixed);
			var tasks = new List<Task>();
			foreach (var f in drives)
			{
				tasks.Add(Task.Factory.StartNew(() =>
				{
					var files = GetFilesAndFolders(f.Name, "eqgame.exe", 2);
					foreach (var item in files)
					{
						try
						{
							var root = Path.GetDirectoryName(item);
							var licensetext = File.ReadAllText(root + "/license.txt");
							if (licensetext.Contains("Project 1999"))
							{
								var dirdata = GetUIFiles(root)
									.OrderByDescending(a => a.LastWriteTime)
									.Select(a => new { a.LastWriteTime, a.FullName })
									.FirstOrDefault();
								if (dirdata != null)
								{
									var directory = new DirectoryInfo(root);
									var maxmoddate = directory.GetFiles()
									.OrderByDescending(a => a.LastWriteTime)
									.Select(a => (DateTime?)a.LastWriteTime)
									.FirstOrDefault();
									if (maxmoddate.HasValue)
									{
										possibles.Enqueue(new Match
										{
											LastModifiedDate = maxmoddate.Value,
											EqBaseLocation = root,
											HasCharUiFiles = false
										});
									}
								}
								else
								{
									possibles.Enqueue(new Match
									{
										LastModifiedDate = dirdata.LastWriteTime,
										EqBaseLocation = root,
										HasCharUiFiles = true
									});
								}
							}
						}
						catch { }
					}
				}));
			}
			var allran = Task.WaitAll(tasks.ToArray(), 1000 * 5);
			var rootfolder = possibles.Where(a => a.HasCharUiFiles).OrderByDescending(a => a.LastModifiedDate).FirstOrDefault();
			if (rootfolder == null)
			{
				rootfolder = possibles.OrderByDescending(a => a.LastModifiedDate).FirstOrDefault();
			}

			if (rootfolder != null)
			{
				var logifles = GetLogFileLocation(new FindEQData
				{
					EqBaseLocation = rootfolder.EqBaseLocation,
					EQlogLocation = rootfolder.EQlogLocation,
				});
				if (logifles.Found)
				{
					rootfolder.EQlogLocation = logifles.Location;
				}
			}

			return rootfolder;
		}
		public static bool IsValidEqFolder(string rootfolder)
		{
			if (string.IsNullOrWhiteSpace(rootfolder))
			{
				return false;
			}

#if QUARM
			return IsQuarmFolder(rootfolder);
#else
            return IsProject1999Folder(rootfolder);
#endif
		}

		private static bool IsQuarmFolder(string rootfolder)
		{
			if (string.IsNullOrWhiteSpace(rootfolder))
			{
				return false;
			}
			var seperator = "\\";
			if (!string.IsNullOrWhiteSpace(rootfolder))
			{
				seperator = rootfolder.Contains("/") ? "/" : "\\";
			}
			if (!rootfolder.EndsWith(seperator))
			{
				rootfolder += seperator;
			}
			try
			{
				return File.Exists(rootfolder + "eqmac.exe");
			}
			catch { }

			return false;
		}

		private static bool IsProject1999Folder(string rootfolder)
		{
			if (string.IsNullOrWhiteSpace(rootfolder))
			{
				return false;
			}

			try
			{
				var licensetext = File.ReadAllText(rootfolder + "/license.txt");
				if (licensetext.Contains("Project 1999"))
				{
					return true;
				}
			}
			catch { }

			return false;
		}

		public static bool? TryCheckLoggingEnabled(string eqdir)
		{
			try
			{
				var data = File.ReadAllLines(eqdir + "/eqclient.ini");
				foreach (var item in data)
				{
					var line = item.ToLower().Trim().Replace(" ", string.Empty);
					if (line.StartsWith("log="))
					{
						return line.Contains("true");
					}
				}
			}
			catch
			{
			}
			return null;
		}

		private static string GetVirtualStoreLocation(string path)
		{
			return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\VirtualStore\\" + path.Substring(3);
		}


		public static FileInfo[] GetUIFiles(string root)
		{
			if (string.IsNullOrWhiteSpace(root))
			{
				return new FileInfo[0];
			}

			try
			{
				var directory = new DirectoryInfo(root);
				var files = directory.GetFiles("UI_*.ini", SearchOption.TopDirectoryOnly);
				if (files.Any())
				{
					return files;
				}
			}
			catch { }

			try
			{
				root = GetVirtualStoreLocation(root);
				var directory = new DirectoryInfo(root);
				var files = directory.GetFiles("UI_*.ini", SearchOption.TopDirectoryOnly);
				if (files.Any())
				{
					return files;
				}
			}
			catch { }

			return new FileInfo[0];
		}

		public static DataFileInfo GetDataLocation(string folderToSearch, string dataFileName, string userFileName)
		{
			DataFileInfo ret = new DataFileInfo { Found = false };
			string dataFolder = Path.Combine(AppContext.BaseDirectory, folderToSearch);
			string dataSearchPattern = dataFileName;
			string userSearchPattern = userFileName;

			try
			{
				var foundFiles = Directory.EnumerateFiles(dataFolder, dataSearchPattern, SearchOption.TopDirectoryOnly);
				if (foundFiles.Any())
				{
					ret.Found = true;
					ret.Location = dataFolder;
					ret.Data_File = Path.Combine(dataFolder, foundFiles.FirstOrDefault());
				}
				foundFiles = Directory.EnumerateFiles(dataFolder, userSearchPattern, SearchOption.TopDirectoryOnly);
				if (foundFiles.Any())
				{
					ret.User_File = Path.Combine(dataFolder, foundFiles.FirstOrDefault());
				}
				return ret;
			}
			catch(DirectoryNotFoundException ex)
			{
				return null;
			}
		}

		public class LogFileInfo
		{
			public string Location { get; set; }
			public bool Found { get; set; }
		}

		public class LogFile
		{
			public string Location { get; set; }
			public string Name { get; set; }
			public string CharName { get; set; }
		}

		public static LogFileInfo GetLogFileLocation(FindEQData data)
		{
			return GetLogFileLocation(data, null);
		}

		public static LogFileInfo GetLogFileLocation(FindEQData data, string selectedCharacter)
		{
			var searchPattern = "eqlog*.txt";
#if QUARM
			searchPattern = "eqlog_*_pq.proj.txt";
#endif
			if (!string.IsNullOrWhiteSpace(selectedCharacter))
			{
				searchPattern = searchPattern.Replace("*", selectedCharacter);
			}

			var seperator = "\\";
			if (!string.IsNullOrWhiteSpace(data.EqBaseLocation))
			{
				seperator = data.EqBaseLocation.Contains("/") ? "/" : "\\";
			}

			var ret = new LogFileInfo { Found = false };
			try
			{
				if (!string.IsNullOrWhiteSpace(data.EQlogLocation))
				{
					ret.Found = Directory.EnumerateFiles(data.EQlogLocation, searchPattern, SearchOption.TopDirectoryOnly).Any();
					if (ret.Found)
					{
						ret.Location = data.EQlogLocation;
						return ret;
					}
				}
			}
			catch { }
			var logs = $"{seperator}Logs{seperator}";
#if QUARM
			logs = seperator;
#endif
			var filepossibles = new List<FileInfo>();
			try
			{
				if (!string.IsNullOrWhiteSpace(data.EqBaseLocation))
				{
					var directory = new DirectoryInfo(data.EqBaseLocation + logs);
					var files = directory.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
					filepossibles.AddRange(files);
				}
			}
			catch { }

			try
			{
				if (!string.IsNullOrWhiteSpace(data.EqBaseLocation))
				{
					var root = GetVirtualStoreLocation(data.EqBaseLocation) + logs;
					var directory = new DirectoryInfo(root);
					var files = directory.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
					filepossibles.AddRange(files);
				}
			}
			catch { }
			var newestfile = filepossibles.OrderByDescending(a => a.LastWriteTime).FirstOrDefault()?.FullName;
			if (!string.IsNullOrWhiteSpace(newestfile))
			{
				ret.Location = Path.GetDirectoryName(newestfile);
				ret.Found = true;
			}
			return ret;
		}

		private static List<string> GetFilesAndFolders(string root, string pathtomatch, int depth)
		{
			var list = new List<string>();
			try
			{
				try
				{
					foreach (var directory in Directory.EnumerateDirectories(root).Where(a => !a.Contains("$")))
					{
						if (depth > 0)
						{
							list.AddRange(GetFilesAndFolders(directory, pathtomatch, depth - 1));
						}
					}
				}
				catch
				{ }
				list.AddRange(Directory.EnumerateFiles(root, pathtomatch));
			}
			catch
			{ }

			return list;
		}
	}
}
