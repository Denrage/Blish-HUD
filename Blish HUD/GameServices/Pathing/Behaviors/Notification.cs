using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Blish_HUD.Controls;
using Blish_HUD.Entities;
using Blish_HUD.Pathing.Behaviors.Activator;
using Microsoft.Xna.Framework;

namespace Blish_HUD.Pathing.Behaviors {

    [PathingBehavior("notification")]
    public class Notification<TPathable, TEntity> : PathingBehavior<TPathable, TEntity>, ILoadableBehavior
        where TPathable : ManagedPathable<TEntity>
        where TEntity : Entity {

        private string _notificationMessage = string.Empty;
        private float _notificationDistance = 2f;
        private ScreenNotification.NotificationType _notificationType = ScreenNotification.NotificationType.Info;

        private bool _canResendMessage = true;

        public string NotificationMessage {
            get => _notificationMessage;
            set => _notificationMessage = value;
        }

        public float NotificationDistance {
            get => _notificationDistance;
            set {
                _notificationDistance = value;
                Load();
            }

        }

        public ScreenNotification.NotificationType NotificationType {
            get => _notificationType;
            set => _notificationType = value;
        }

        /// <inheritdoc />
        public Notification(TPathable managedPathable) : base(managedPathable) { }

        /// <inheritdoc />
        public override void Load() {
            if (this.Activator != null) {
                this.Activator.Activated   -= ActivatorOnActivated;
                this.Activator.Deactivated -= ActivatorOnDeactivated;

                this.Activator.Dispose();
            }

            this.Activator = new ZoneActivator<TPathable, TEntity>(this) {
                ActivationDistance = _notificationDistance,
                DistanceFrom       = DistanceFrom.Player
            };

            this.Activator.Activated   += ActivatorOnActivated;
            this.Activator.Deactivated += ActivatorOnDeactivated;
        }

        private void ActivatorOnActivated(object sender, EventArgs e) {
            if (_canResendMessage)
                Controls.ScreenNotification.ShowNotification(_notificationMessage, _notificationType);

            _canResendMessage = false;
        }

        private void ActivatorOnDeactivated(object sender, EventArgs e) {
            _canResendMessage = true;
        }

        /// <inheritdoc />
        public void LoadWithAttributes(IEnumerable<PathableAttribute> attributes) {
            foreach (var attr in attributes) {
                switch (attr.Name.ToLowerInvariant()) {
                    case "notification":
                        _notificationMessage = attr.Value;
                        break;
                    case "notification-distance":
                        InvariantUtil.TryParseFloat(attr.Value, out _notificationDistance);
                        break;
                    case "notification-type":
                        Enum.TryParse(attr.Value, true, out _notificationType);
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
