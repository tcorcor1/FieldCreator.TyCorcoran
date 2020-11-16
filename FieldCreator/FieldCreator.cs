using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace FieldCreator.TyCorcoran
{
    // To generate Base64 string for Images below, you can use https://www.base64-image.de/
    [Export(typeof(IXrmToolBoxPlugin)),
        ExportMetadata("Name", "Field Creator"),
        ExportMetadata("Description", "Bulk create your fields by importing a csv. Supports all types."),
        // Please specify the base64 content of a 32x32 pixels image
        ExportMetadata("SmallImageBase64", "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAABg2lDQ1BJQ0MgcHJvZmlsZQAAKM+VkTlIA0EYhT9jRPEsjCBisYVaKYiKWEoURVCQGCEehbsbE4XsJuwm2FgKtgELj8arsLHW1sJWEAQPECtLK0UbkfWfjZAgRHBgmI838x4zbyBwkDItN9gLlp11IuNhLTY3r1U/E6SNFupo1E03MzUzFqXs+LilQq03PSqL/42G+LJrQoUmPGxmnKzwkvDgWjajeEc4ZK7oceFT4W5HLih8r3SjwC+Kkz4HVGbIiUZGhEPCWrKEjRI2VxxLeEC4I27Zkh+IFTiueF2xlcqZP/dUL6xftmdnlC6znXEmmGIaDYMcq6TI0iOrLYpLRPbDZfxtvn9aXIa4VjHFMUoaC933o/7gd7duor+vkFQfhqonz3vrhOot+Mp73ueh530dQeUjXNhFf/oAht5Fzxe1jn1o2oCzy6JmbMP5JrQ+ZHRH96VKmYFEAl5P5JvmoPkaahcKvf3sc3wHUelq8gp296ArKdmLZd5dU9rbn2f8/gh/A0hycpajSMXPAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAAB3RJTUUH5AQaFRIRg4zTtQAAA3FJREFUWEfFVk1IVFEUPs+f+Z/R8S9bCElBual0ZAh0diG2MgPRFiEObapFi2jTNqJsZ1ELcQRrEQwUQtAiiCBm694UWkQojX9DOGLz4+07t/tgfL73Zt5o+cHh3vPuve9855x7z70aEQnIsaFGtceGQxHorqtTvcOBU+BYhl0uIdrbTcecSNURGHW7ifb2lFY9qt6EorVVttrammyrRVURuMS515i7NWKxGEWjUaXZ40BeykkyGBSIgBSzcRZGMpk0HSuVqiIw4vXSruD15hgaGpJtJpORrR0cE5jgzQfj1uaJ5ufnVa88HBN4HAhQ3sb7RCKhepXBEQEuPCdqaymvdCN6enooHo8rzRrrzc303O+XfUcEpuF90ebsLywsqJ41PoRC1AwndFRMoLOmhnpdLvqtdCNyuZzqWeNVMEhX1B7S3aiYwDswt/I+lUpRfX290szxBsZveDy0Y9g/FRHoRe4v2ngfxM/NoJtKNTbSmIlxRkUEPjU0UM4m98ViUfX2Q//aB/JmxhllCTxA0Qkh/wWlHzVsCQRQ7x8hvFbsjwK2BL43NVHeIvRHRcmSwFvs+jBCb1Z0vBAf34bqRtyzIllB5EwJ3Eferxl2LZcONuoDqY848wN4B2jptByLRCKyNUIrc2UzDhAYR6F4ynlXXvHpZsO8o29tbUmjg79+0Sru+umZGVsvvXCkHPa9Km/D6xcIPRtnj90wnEZ/bH2dPkPXcBxnp6ZoYnxczteRBqmVlRXaBMEtXMHZbJb8KNvTc3MU4AllUsGj4jU/MtraRLal5e9jA+1VPDx5rLuvT6DaiaXlZfFwclL0DwwIF8b1tVaC8IqlcFjkMZf/qwv/f8rvl3PkmzABtnGfj3bhrQc5frmzQ3e2tzFEdLKzE6Fw0+riotTZo35IBKX3PL6fQZXswOXSinVmz7QC/mm8JTilz2DjLiIlCcyCwITKV9fmJi2WVLZzkOuoZCO4Pru43rOR0pCahJf3BRculoOjJgTmQGAY3oQ2NuSEU/DmCQyOIirSILz4VijQEoj9gPyEnoGRbYj8AcSDeU2QNqztgJxGVM7qj1dFkontovUaCchR4AIW3YPRRkx4j6P2JZ+nrxZ13gmiIBJD9C5DBvk6BsEEjN9UaWYCUnjTlOr/SuCoCGua7O+LwHHAtBL+PxD9AXfom/MIW7MGAAAAAElFTkSuQmCC"),
        // Please specify the base64 content of a 80x80 pixels image
        ExportMetadata("BigImageBase64", "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"),
        ExportMetadata("BackgroundColor", "Black"),
        ExportMetadata("PrimaryFontColor", "White"),
        ExportMetadata("SecondaryFontColor", "Red")]
    public class FieldCreator : PluginBase
    {
        public override IXrmToolBoxPluginControl GetControl()
        {
            return new FieldCreatorPluginControl();
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        public FieldCreator()
        {
            // If you have external assemblies that you need to load, uncomment the following to 
            // hook into the event that will fire when an Assembly fails to resolve
            // AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named Sample.XrmToolBox.FieldCreator 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;
            Assembly currAssembly = Assembly.GetExecutingAssembly();

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(","));

            // check to see if the failing assembly is one that we reference.
            List<AssemblyName> refAssemblies = currAssembly.GetReferencedAssemblies().ToList();
            var refAssembly = refAssemblies.Where(a => a.Name == argName).FirstOrDefault();

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (refAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                string dir = Path.GetDirectoryName(currAssembly.Location).ToLower();
                string folder = Path.GetFileNameWithoutExtension(currAssembly.Location);
                dir = Path.Combine(dir, folder);

                var assmbPath = Path.Combine(dir, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                }
                else
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {assmbPath}");
                }
            }

            return loadAssembly;
        }
    }
}