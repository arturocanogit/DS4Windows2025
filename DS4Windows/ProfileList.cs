/*
DS4Windows
Copyright (C) 2023  Travis Nickles

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DS4WinWPF.Infraestructura;

namespace DS4WinWPF
{
    public class ProfileList
    {
        private object _proLockobj = new object();
        private ObservableCollection<ProfileEntity> profileListCol =
            new ObservableCollection<ProfileEntity>();

        public ObservableCollection<ProfileEntity> ProfileListCol { get => profileListCol; set => profileListCol = value; }

        public ProfileList()
        {
            BindingOperations.EnableCollectionSynchronization(profileListCol, _proLockobj);
        }

        public void Refresh()
        {
            profileListCol.Clear();
            
            if (FtpConfig.EnableFtpUpload)
            {
                try
                {
                    var ftpManager = new FtpManager(FtpConfig.Server, FtpConfig.Username, FtpConfig.Password);
                    var profileFiles = ftpManager.ListProfileFiles();
                    
                    foreach (string fileName in profileFiles)
                    {
                        if (fileName.EndsWith(".cif"))
                        {
                            ProfileEntity item = new ProfileEntity()
                            {
                                Name = Path.GetFileNameWithoutExtension(fileName)
                            };
                            profileListCol.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //AppLogger.LogToGui($"Error loading profile list from FTP: {ex.Message}", true);
                }
            }
        }

        public void AddProfileSort(string profilename)
        {
            int idx = 0;
            bool inserted = false;
            foreach (ProfileEntity entry in profileListCol)
            {
                if (entry.Name.CompareTo(profilename) > 0)
                {
                    profileListCol.Insert(idx, new ProfileEntity() { Name = profilename });
                    inserted = true;
                    break;
                }
                idx++;
            }

            if (!inserted)
            {
                profileListCol.Add(new ProfileEntity() { Name = profilename });
            }
        }

        public void RemoveProfile(string profile)
        {
            var selectedEntity = profileListCol.SingleOrDefault(x => x.Name == profile);
            if (selectedEntity != null)
            {
                int selectedIndex = profileListCol.IndexOf(selectedEntity);
                profileListCol.RemoveAt(selectedIndex);
            }
        }
    }
}
