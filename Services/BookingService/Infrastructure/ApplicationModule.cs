using Autofac;
using BookingService.Interfaces;
using BookingService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Infrastructure
{
    public class ApplicationModule: Autofac.Module
    {
        public ApplicationModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BookingRepository>()
                .As<IBookingRepository>()
                .InstancePerLifetimeScope();
        }
    }
}
