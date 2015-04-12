using System.Drawing;
using System.Windows.Forms;

namespace consoleXstream.Home
{
    public class Mouse
    {
        public Mouse(Classes classes) { _class = classes; }
        private readonly Classes _class;

        private int _mouseX;
        private int _mouseY;


        public void Check()
        {
            if (_class.BaseClass.System.IsMouseDirect)
                CheckDirectMovement();
            else
                CheckCircularMovement();
        }

        private void CheckDirectMovement()
        {
            _class.Var.MouseX = Clamp((int)((_mouseX - Cursor.Position.X) * -_class.BaseClass.System.MouseModifierX), -100, 100);
            _class.Var.MouseY = Clamp((int)((_mouseY - Cursor.Position.Y) * -_class.BaseClass.System.MouseModifierY), -100, 100);

            _mouseX = Cursor.Position.X;
            _mouseY = Cursor.Position.Y;

            if (_class.BaseClass.System.IsMouseFreeRoll) CheckMouseCenter();
        }


        private void CheckCircularMovement()
        {
            
        }

        private void CheckMouseCenter()
        {
            if (Cursor.Position.Y > Screen.PrimaryScreen.Bounds.Height - _class.Var.MouseScreenBounds)
                CenterMouseY();

            if (Cursor.Position.Y < _class.Var.MouseScreenBounds)
                CenterMouseY();

            if (Cursor.Position.X < _class.Var.MouseScreenBounds)
                CenterMouseX();

            if (Cursor.Position.X > Screen.PrimaryScreen.Bounds.Width - _class.Var.MouseScreenBounds)
                CenterMouseX();
        }

        private void CenterMouseY()
        {
            Cursor.Position = new Point(Cursor.Position.X, Screen.PrimaryScreen.Bounds.Height / 2);
            _mouseY = Cursor.Position.Y;
        }

        private void CenterMouseX()
        {
            Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Cursor.Position.Y);
            _mouseX = Cursor.Position.X;
        }

        public int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
