using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LogicMatrix;
using Microsoft.Xna.Framework.Storage;

namespace Happiness
{
    public class SaveGame
    {
        public bool m_bIsValid = false;
        private StorageDevice m_Storage;
        const int m_iVersion = 0;
        const int m_iOptionsVersion = 1;

#if !XBOX
        public SaveGame()
        {
            try
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Happiness");

                m_bIsValid = DoesSaveGameExist(false);
            }
            catch (Exception)
            {
            }
        }
#endif

        public SaveGame(StorageDevice device)
        {
            m_Storage = device;
            m_bIsValid = DoesSaveGameExist(false);
        }

        public void LoadOptions(out bool bAutoArangeClues, out bool bShowClueDescriptions, out bool bShowClock, out bool bShowPuzzleNumber, out bool bRandomizeClues, out float fSoundVolume, out float fMusicVolume)
        {
            // Set Defaults
            bAutoArangeClues = false;
            bShowClueDescriptions = true;
            bShowClock = true;
            bShowPuzzleNumber = true;
            bRandomizeClues = true;
            fSoundVolume = 0.8f;
            fMusicVolume = 0.8f;

            // Try to load from options file
            #if XBOX
            StorageContainer container = m_Storage.OpenContainer("Happiness");
            string filename = Path.Combine(container.Path, "Happiness.opt");
            #else
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Happiness\\Happiness.opt";
            #endif

            try
            {
                BinaryReader br = new BinaryReader(File.OpenRead(filename));

                int iVersion = br.ReadInt32();
                if (iVersion == m_iOptionsVersion)
                {
                    bAutoArangeClues = br.ReadBoolean();
                    bShowClueDescriptions = br.ReadBoolean();
                    bShowClock = br.ReadBoolean();
                    bShowPuzzleNumber = br.ReadBoolean();
                    bRandomizeClues = br.ReadBoolean();
                    fSoundVolume = br.ReadSingle();
                    fMusicVolume = br.ReadSingle();
                }

                br.Close();
            }
            catch (Exception)
            {
            }

#if XBOX
            if (container != null)
                container.Dispose();
#endif
        }

        public void SaveOptions(bool bAutoArangeClues, bool bShowClueDescriptions, bool bShowClock, bool bShowPuzzleNumber, bool bRandomizeClues, float fSoundVol, float fMusicVol)
        {
            #if XBOX
            StorageContainer container = m_Storage.OpenContainer("Happiness");
            string filename = Path.Combine(container.Path, "Happiness.opt");
            #else
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Happiness\\Happiness.opt";
            #endif

            try
            {
                BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create, FileAccess.Write));

                bw.Write(m_iOptionsVersion);
                bw.Write(bAutoArangeClues);
                bw.Write(bShowClueDescriptions);
                bw.Write(bShowClock);
                bw.Write(bShowPuzzleNumber);
                bw.Write(bRandomizeClues);
                bw.Write(fSoundVol);
                bw.Write(fMusicVol);
                bw.Close();
            }
            catch (Exception)
            {
            }

#if XBOX
            if (container != null)
                container.Dispose();
#endif
        }

        public bool LoadPuzzle(out Puzzle P, out int iDifficulty, out int iPuzzleNumber)
        {
            bool bSuccess = false;

            P = null;
            iDifficulty = 0;
            iPuzzleNumber = 0;
            if (m_bIsValid)
            {
                #if XBOX
                StorageContainer container = m_Storage.OpenContainer("Happiness");
                string filename = Path.Combine(container.Path, "Happiness.sav");
                #else
                string filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Happiness\\Happiness.sav";
                #endif

                try
                {
                    BinaryReader br = new BinaryReader(File.OpenRead(filename));

                    int iVersion = br.ReadInt32();
                    if (iVersion == m_iVersion)
                    {
                        int iSeed = br.ReadInt32();
                        iDifficulty = iSeed & 0x3;
                        int iSize = ((iSeed >> 2) & 0x7) + 3;
                        iPuzzleNumber = iSeed;
                        P = new Puzzle(iSeed, iSize, iDifficulty);
                        for (int iRow = 0; iRow < P.m_iSize; iRow++)
                        {
                            for (int iCol = 0; iCol < P.m_iSize; iCol++)
                            {
                                for (int iIcon = 0; iIcon < P.m_iSize; iIcon++)
                                {
                                    P.m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon] = br.ReadBoolean();
                                }
                                P.m_Rows[iRow].m_Cells[iCol].m_iFinalIcon = P.m_Rows[iRow].m_Cells[iCol].GetRemainingIcon();
                            }
                        }

                        bSuccess = true;
                    }

                    br.Close();                    
                }
                finally
                {
#if XBOX
                    if (container != null)
                        container.Dispose();
#endif
                }
            }

            return bSuccess;
        }

        public bool SavePuzzle(Puzzle P)
        {
            bool bSuccess = false;
            if (DoesSaveGameExist(true))
            {
                #if XBOX
                StorageContainer container = m_Storage.OpenContainer("Happiness");
                string filename = Path.Combine(container.Path, "Happiness.sav");
                #else
                string filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Happiness\\Happiness.sav";
                #endif

                try
                {
                    BinaryWriter bw = new BinaryWriter(File.OpenWrite(filename));

                    bw.Write(m_iVersion);
                    bw.Write(P.m_iSeed);
                    for (int iRow = 0; iRow < P.m_iSize; iRow++)
                    {
                        for (int iCol = 0; iCol < P.m_iSize; iCol++)
                        {
                            for (int iIcon = 0; iIcon < P.m_iSize; iIcon++)
                            {
                                bw.Write(P.m_Rows[iRow].m_Cells[iCol].m_bValues[iIcon]);
                            }
                        }
                    }
                    bw.Close();
                    bSuccess = true;
                    m_bIsValid = true;
                }
                finally
                {
#if XBOX
                    if (container != null)
                        container.Dispose();
#endif
                }
            }
            return bSuccess;
        }

        private bool DoesSaveGameExist(bool bCreate)
        {
            bool bExists = false;
            string filename = null;

            #if XBOX
            StorageContainer container = null;
            if (m_Storage.IsConnected)
            {
                container = m_Storage.OpenContainer("Happiness");
                filename = Path.Combine(container.Path, "Happiness.sav");
            }
            #else
            filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Happiness\\Happiness.sav";
            #endif

            if ( filename != null )
            {
                try
                {
                    FileStream file = File.Open(filename, FileMode.Open);
                    file.Close();
                    bExists = true;
                }
                catch (FileNotFoundException)
                {
                    if (bCreate)
                    {
                        FileStream file = File.Create(filename);
                        file.Close();
                        bExists = true;
                    }
                }
                catch (Exception)
                {
                }

                #if XBOX
                if (container != null)
                    container.Dispose();
                #endif
            }
            return bExists;
        }
    }
}
