

namespace Metatrader4ClientApp.Infrastructure.Interfaces
{
    using Metatrader4ClientApp.Infrastructure.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    /// <summary>
    /// Provides an easy way to recognize a class that exposes a HeaderInfo that can be used to bind to a header from XAML.
    /// </summary>
    /// <typeparam name="T">The HeaderInfo type</typeparam>
    public interface IExportService
    {
       

        /// <summary>
        /// Export objects from type <see cref="AccountPosition"/> to a file.
        /// </summary>
        /// <param name="collection">Objects as <see cref="IEnumerable{AccountPosition}"/> to export.</param>
        /// <param name="filePath">Path to the export file.</param>
        /// <param name="fileType">Allowed <see cref="ExportFileType"/> are CSV, XML or JSON.</param>

        bool Export(IEnumerable<AccountPosition> accountPositions, string filePath, ExportFileType fileType);

        void ExportToTextFile<T>(IEnumerable<T> data, string fileName, char columnSeperator = ';');
        Task ExportToTextFileAsync<T>(IEnumerable<T> data, string fileName, char columnSeperator = ';');
    }
}
