using Notist.Core.Models.Collections;

namespace Notist.Models
{
    public class ApplicationContext
    {
        public ICollection Collection { get; set; }

        public static ApplicationContext Current => new ApplicationContext();
    }
}
