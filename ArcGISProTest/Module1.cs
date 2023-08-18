using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Events;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Editing.Events;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ArcGISProTest
{
    internal class Module1 : Module
    {
        private static Module1 _this = null;

        /// <summary>
        /// Retrieve the singleton instance to this module here
        /// </summary>
        public static Module1 Current => _this ??= (Module1)FrameworkApplication.FindModule("ArcGISProTest_Module");

        #region Overrides
        /// <summary>
        /// Called by Framework when ArcGIS Pro is closing
        /// </summary>
        /// <returns>False to prevent Pro from closing, otherwise True</returns>
        protected override bool CanUnload()
        {
            //TODO - add your business logic
            //return false to ~cancel~ Application close
            return true;
        }

        #endregion Overrides

        protected override bool Initialize()
        {
            ProjectOpenedEvent.Subscribe(OnProjectLoaded);
            return base.Initialize();
        }

        private void OnProjectLoaded(ProjectEventArgs args)
        {
            var mapItem = Project.Current.GetItems<MapProjectItem>().FirstOrDefault(x => x.Name == "AMFM Editor");
            if (mapItem == null) return;
            QueuedTask.Run(() =>
            {
                var map = mapItem.GetMap();

                var layers = map.GetLayersAsFlattenedList();

                foreach (var layer in layers)
                {
                    if (layer is AnnotationLayer)
                    {
                        var annotationLayer = (AnnotationLayer)layer;
                        RowCreatedEvent.Subscribe(OnAnnoCreated, annotationLayer.GetTable());
                        RowChangedEvent.Subscribe(OnAnnoUpdated, annotationLayer.GetTable());

                    }
                }
            });
        }

        private void OnAnnoCreated(RowChangedEventArgs args)
        {
            MessageBox.Show("Annotation Created");
        }

        private void OnAnnoUpdated(RowChangedEventArgs args)
        {
            MessageBox.Show("Annotation Updated");
        }
    }
}
