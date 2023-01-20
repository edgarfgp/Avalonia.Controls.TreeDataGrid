﻿using Avalonia.Media;

namespace Avalonia.Controls.Models.TreeDataGrid
{
    /// <summary>
    /// Represents a text cell in an <see cref="ITreeDataGridSource"/>.
    /// </summary>
    public interface ITextCell : ICell
    {
        /// <summary>
        /// Gets the cell's text trimming mode.
        /// </summary>
        TextTrimming TextTrimming { get; }

        /// <summary>
        /// Gets the cell's text alignment mode.
        /// </summary>
        public TextAlignment TextAlignment { get; }
    }
}
