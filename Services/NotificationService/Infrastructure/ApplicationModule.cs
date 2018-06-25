using Autofac;
using NotificationService.Interfaces;
using NotificationService.Repositories;

namespace NotificationService.Infrastructure
{
    public class ApplicationModule : Autofac.Module
    {
        public ApplicationModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PushSubscriptionRepository>()
                .As<IPushSubscriptionRepository>()
                .InstancePerLifetimeScope();
        }
    }
}
