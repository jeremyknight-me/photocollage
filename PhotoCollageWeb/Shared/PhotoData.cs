namespace PhotoCollageWeb.Shared
{
    public class PhotoData
    {
        public PhotoData()
        {
            this.Id = Guid.NewGuid();
            this.IsRemoved = false;
        }

        public Guid Id { get; init; }
        public string Data { get; init; }
        public string Extension { get; init; }
        public bool IsRemoved { get; set; }

        //private readonly int index;

        //public PhotoData(int count, CollageSettings settings)
        //{
        //    this.index = count;
        //    this.DisplayCssStyles = this.CombineStyles(this.InitializeDisplayStyles(settings));
        //    this.PositionCssStyles = this.CombineStyles(this.InitializePositionStyles(settings));
        //}

        //public string DisplayCssStyles { get; }
        //public string PositionCssStyles { get; }

        //private string CombineStyles(IDictionary<string, string> styles) => string.Join(";", styles.OrderBy(x => x.Key).Select(x => $"{x.Key}:{x.Value}"));

        //private IDictionary<string, string> InitializeDisplayStyles(CollageSettings settings)
        //{
        //    var styles = new Dictionary<string, string>
        //    {
        //        { "max-height", $"{settings.MaximumSize}px" },
        //        { "max-width", $"{settings.MaximumSize}px" }
        //    };
        //    if (settings.IsGrayscale)
        //    {
        //        styles.Add("filter", "grayscale(1)");
        //    }
        //    return styles;
        //}

        //private IDictionary<string, string> InitializePositionStyles(CollageSettings settings)
        //{
        //    var random = new Random();
        //    var positionTop = random.Next(0, 100);
        //    var positionLeft = random.Next(0, 100);
        //    var rotation = random.Next(-settings.MaximumRotation, settings.MaximumRotation);
        //    var half = settings.MaximumSize / 2;

        //    return new Dictionary<string, string>
        //    {
        //        { "left", $"calc({positionLeft}vw - {half}px)" },
        //        { "top", $"calc({positionTop}vh - {half}px)" },
        //        { "transform", $"rotate({rotation}deg)" },
        //        { "z-index", $"{this.index}" }
        //    };
        //}
    }
}
