using System;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using PhotoCollage.Common;
using PhotoCollageWeb.Models;

namespace PhotoCollageWeb.Components
{
    public partial class PhotoFrame
    {
        private int positionLeft = 0;
        private int positionTop = 0;
        private int rotation = 0;

        [Parameter] public ImageData Image { get; set; }
        [Inject] protected IOptions<CollageSettings> Options { get; set; }
        protected CollageSettings Settings => this.Options.Value;

        protected string CssClasses => "photo-frame bordered";

        protected string CssStyles
        {
            get
            {
                var size = this.Settings.MaximumSize;
                var half = size / 2;
                var styles = $"left:calc({this.positionLeft}vw - {half}px);top:calc({this.positionTop}vh - {half}px);transform:rotate({this.rotation}deg);z-index:{this.Image.Count}";
                return styles;
            }
        }

        protected override void OnInitialized()
        {
            var random = new Random();
            this.positionTop = random.Next(0, 100);
            this.positionLeft = random.Next(0, 100);
            this.rotation = random.Next(-this.Settings.MaximumRotation, this.Settings.MaximumRotation);
            base.OnInitialized();
        }
    }
}
