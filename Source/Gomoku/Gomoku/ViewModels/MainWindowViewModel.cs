using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace gomoku.ViewModels
{
    class MainWindowViewModel
    {
        public int x { get; set; } //Tọa độ dòng
        public int y { get; set; } //Tọa độ cột
        public Point _p;
        public Point p
        {
            get { return _p; }
            set
            {
                if (value != _p)
                    _p = value;
            }
        }
        public ImageBrush _image;
        public ImageBrush image
        {
            get { return _image; }
            set 
            {
                if(value != _image)
                {
                    _image = value;
                    if (_click != null)
                        _click(_image, p);
                }
            }
        }
        public delegate void ChangedClickHandler(ImageBrush new_value, Point point);
        public event ChangedClickHandler _click;


    }
}
