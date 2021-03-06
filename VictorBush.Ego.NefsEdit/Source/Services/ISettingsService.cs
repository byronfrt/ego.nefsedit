﻿// See LICENSE.txt for license information.

namespace VictorBush.Ego.NefsEdit.Services
{
    /// <summary>
    /// Settings service.
    /// </summary>
    internal interface ISettingsService
    {
        /// <summary>
        /// Gets or sets the DiRT 4 directory.
        /// </summary>
        string Dirt4Dir { get; set; }

        /// <summary>
        /// Gets the DiRT 4 executable.
        /// </summary>
        string Dirt4Exe { get; }

        /// <summary>
        /// Gets the directory that contains game.dat files.
        /// </summary>
        string Dirt4GameDatDir { get; }

        /// <summary>
        /// Gets or sets the DiRT Rally 2 directory.
        /// </summary>
        string DirtRally2Dir { get; set; }

        /// <summary>
        /// Gets the DiRT Rally 2 executable.
        /// </summary>
        string DirtRally2Exe { get; }

        /// <summary>
        /// Gets the directory that contains game.dat files.
        /// </summary>
        string DirtRally2GameDatDir { get; }

        /// <summary>
        /// Gets or sets the quick extract directory.
        /// </summary>
        string QuickExtractDir { get; set; }

        /// <summary>
        /// Shows a dialog to choose the quick extract dir.
        /// </summary>
        /// <returns>True if the directory was chosen.</returns>
        bool ChooseQuickExtractDir();

        /// <summary>
        /// Loads settings from disk or loads defaults.
        /// </summary>
        void Load();

        /// <summary>
        /// Saves the settings file to disk.
        /// </summary>
        void Save();
    }
}
