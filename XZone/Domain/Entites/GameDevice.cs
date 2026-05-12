using System.ComponentModel.DataAnnotations.Schema;

namespace XZone.Domain.Entites
{
    public class GameDevice
    {
        [ForeignKey("Game")]
        public int GameId { get; set; }

        public Game Game { get; set; }

        [ForeignKey("Device")]
        public int DeviceId { get; set; }

        public Device Device { get; set; }
    }
}
