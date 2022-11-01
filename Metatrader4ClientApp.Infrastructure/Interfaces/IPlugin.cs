using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metatrader4ClientApp.Infrastructure.Interfaces
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Defines the <see cref="IPlugin"/>.
    /// </summary>
    public interface IPlugin 
    {
        /// <summary>
        /// Gets or sets the Code.
        /// </summary>
        string Code { get; set; }

        /// <summary>
        /// Gets the Header.
        /// </summary>
        string Header { get; }


        /// <summary>
        /// Gets or sets the Command.
        /// </summary>
        ICommand Command { get; set; }

    /// <summary>
    /// Gets or sets the CommandParameter.
    /// </summary>
    Type CommandParameter { get; set; }

    /// <summary>
    /// Gets or sets the Description.
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Gets or sets the Glyph.
    /// </summary>
    string Glyph { get; set; }

    /// <summary>
    /// Gets or sets the Id.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether IsSelected.
    /// </summary>
    bool IsSelected { get; set; }

    /// <summary>
    /// Gets or sets the Kind To interact with Material Design PackIcon.
    /// </summary>
    string Kind { get; set; }

    /// <summary>
    /// Gets or sets the Label.
    /// </summary>
    string Label { get; set; }

    /// <summary>
    /// Gets or sets the NavigatePath.
    /// </summary>
    string NavigatePath { get; set; }

    /// <summary>
    /// Gets or sets the Tag.
    /// </summary>
    string Tag { get; set; }

    /// <summary>
    /// Gets or sets the Title.
    /// </summary>
    string Title { get; set; }
}
}
