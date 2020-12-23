﻿using System;
using System.Collections.Generic;
using System.Linq;
using PhotoCollage.Common;

namespace PhotoCollageWeb.Models
{
    public class PhotoData
    {
        private readonly int index;
        private readonly IDictionary<string, string> displayStyles;
        private readonly IDictionary<string, string> positionStyles;

        public PhotoData(int count, CollageSettings settings)
        {
            this.index = count;
            this.Key = Guid.NewGuid();
            this.displayStyles = this.InitializeDisplayStyles(settings);
            this.positionStyles = this.InitializePositionStyles(settings);
        }

        public Guid Key { get; }
        public string Data { get; init; }
        public string Extension { get; init; }

        public string DisplayCssStyles => this.CombineStyles(this.displayStyles);
        public string PositionCssStyles => this.CombineStyles(this.positionStyles);

        private string CombineStyles(IDictionary<string, string> styles) => string.Join(";", styles.OrderBy(x => x.Key).Select(x => $"{x.Key}:{x.Value}"));

        private IDictionary<string, string> InitializeDisplayStyles(CollageSettings settings)
        {
            var styles = new Dictionary<string, string>
            {
                { "max-height", $"{settings.MaximumSize}px" },
                { "max-width", $"{settings.MaximumSize}px" }
            };
            if (settings.IsGrayscale)
            {
                styles.Add("filter", "grayscale(1)");
            }
            return styles;
        }

        private IDictionary<string, string> InitializePositionStyles(CollageSettings settings)
        {
            var random = new Random();
            var positionTop = random.Next(0, 100);
            var positionLeft = random.Next(0, 100);
            var rotation = random.Next(-settings.MaximumRotation, settings.MaximumRotation);
            var half = settings.MaximumSize / 2;

            return new Dictionary<string, string>
            {
                { "left", $"calc({positionLeft}vw - {half}px)" },
                { "top", $"calc({positionTop}vh - {half}px)" },
                { "transform", $"rotate({rotation}deg)" },
                { "z-index", $"{this.index}" }
            };
        }
    }
}
