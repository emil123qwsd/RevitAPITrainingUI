using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITrainingUI
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;

        //public DelegateCommand SelectCommand { get; }

        public DelegateCommand SaveCommand { get; }

        public List<Element> PickedObjects { get; } = new List<Element>();

        public List<WallFunction> WallSystems { get; } = new List<WallFunction>();

        public WallType SelectedWallSystem { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            SaveCommand = new DelegateCommand(OnSaveCommand);
            PickedObjects = SelectionUtils.PickObjects(commandData);
            WallSystems = WallsUtils.GetWallSystems(commandData); 
            //SelectCommand = new DelegateCommand(OnSelectCommand);
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (PickedObjects.Count == 0 || SelectedWallSystem == null)
                return;

            using (var ts = new Transaction(doc, "Set system type"))
            {
                ts.Start();

                foreach (var pickedObject in PickedObjects)
                {
                    if (pickedObject is Wall)
                    {
                        var oWall = pickedObject as Wall;
                        oWall.SetSystemType(SelectedWallSystem.Id);              
                    }
                }

                ts.Commit();
            }
            RaiseCloseRequest();
        }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest() 
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        //public event EventHandler HideRequest;

        //private void RaiserHideRequest() 
        //{
        //    HideRequest?.Invoke(this, EventArgs.Empty);
        //}

        //public event EventHandler ShowRequest;

        //private void RaiseShowRequest()
        //{
        //    ShowRequest?.Invoke(this, EventArgs.Empty);
        //}

        //private void OnSelectCommand()
        //{
        //    RaiserHideRequest();
        //    Element oElement = SelectionUtils.PickObject(_commandData);

        //    TaskDialog.Show("Сообщение", $"ID: {oElement.Id}");

        //    RaiseShowRequest();
        //}

    }
}
