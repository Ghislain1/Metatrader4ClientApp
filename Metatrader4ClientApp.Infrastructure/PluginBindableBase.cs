

namespace Metatrader4ClientApp.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Metatrader4ClientApp.Infrastructure.Interfaces;

    using Prism.Mvvm;
    public abstract class PluginBindableBase : BindableBase, IPlugin
    {

        /// <summary>
        /// Gets or sets the Command.
        /// </summary>
        public ICommand? Command { get; set; }

        /// <summary>
        /// Gets or sets the CommandParameter.
        /// </summary>
        public Type CommandParameter { get; set; }

        /// <summary>
        /// Gets or sets the Code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets the Header.
        /// </summary>
        public string Header { get; } = "Header";

        /// <summary>
        /// Gets or sets a value indicating whether IsSelected.
        /// </summary>
        public bool IsSelected { get; set; } = true;


        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public string Description { get; set; } = "Add description";

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the Kind To interact with Material Design PackIcon.
        /// </summary>
        public string PackIcon { get; set; } = "Magnify";

        /// <summary>
        /// Gets or sets the Label.
        /// </summary>
        public string Label { get; set; } = "Add label";

        /// <summary>
        /// Gets or sets the NavigatePath.
        /// </summary>
        public string NavigatePath { get; set; }

        /// <summary>
        /// Gets or sets the Tag.
        /// </summary>
        public string Tag { get; set; } = "Add tag";

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title { get; set; } = "Add Title";




    }
}
