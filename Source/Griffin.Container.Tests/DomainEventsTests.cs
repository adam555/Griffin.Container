﻿using System;
using Griffin.Container.DomainEvents;
using Xunit;

namespace Griffin.Container.Tests
{

    public class DomainEventsTests
    {
        [Fact]
        public void InvokeTwoSubscribers()
        {
            var registrar = new ContainerRegistrar();
            registrar.RegisterConcrete<Subscriber1>();
            registrar.RegisterConcrete<Subscriber2>();
            var container = registrar.Build();

            using(var scope = container.CreateChildContainer())
            {
                DomainEvent.Publish("Hello world");

                var service1 = scope.Resolve<IConcrete1>();
                Assert.True(service1.Handled);
                var service2 = scope.Resolve<IConcrete2>();
                Assert.True(service2.Handled);
            }

        }

        [Fact]
        public void NoScope()
        {
            var registrar = new ContainerRegistrar();
            registrar.RegisterConcrete<Subscriber1>();
            registrar.RegisterConcrete<Subscriber2>();
            var container = registrar.Build();

            Assert.Throws<InvalidOperationException>(() => DomainEvent.Publish("Hello world"));
        }

        [Fact]
        public void DoubleScope()
        {
            var registrar = new ContainerRegistrar();
            registrar.RegisterConcrete<Subscriber1>();
            registrar.RegisterConcrete<Subscriber2>();
            var container = registrar.Build();

            using (var scope = container.CreateChildContainer())
            {
                using (var scope2 = container.CreateChildContainer())
                {
                    DomainEvent.Publish("Hello world");

                    var service3 = scope2.Resolve<IConcrete1>();
                    Assert.True(service3.Handled);
                    var service4 = scope2.Resolve<IConcrete2>();
                    Assert.True(service4.Handled);
                }

                DomainEvent.Publish("Hello world");
                var service1 = scope.Resolve<IConcrete1>();
                Assert.True(service1.Handled);
                var service2 = scope.Resolve<IConcrete2>();
                Assert.True(service2.Handled);
            }

            Assert.Throws<InvalidOperationException>(() => DomainEvent.Publish("Hello world"));

        }

        interface IConcrete1 : ISubscriberOf<string>
        {
            bool Handled { get; }
             
        }

        interface IConcrete2 : ISubscriberOf<string>
        {
            bool Handled { get; }
        }
        class Subscriber1 : ISubscriberOf<string>, IConcrete1
        {
            /// <summary>
            /// Handle the domain event
            /// </summary>
            /// <typeparam name="T">Type of event</typeparam>
            /// <param name="e">The event</param>
            public void Handle(string e)
            {
                Handled = true;
            }

            public bool Handled { get; set; }
        }

        class Subscriber2 :ISubscriberOf<string>, IConcrete2
        {
            /// <summary>
            /// Handle the domain event
            /// </summary>
            /// <typeparam name="T">Type of event</typeparam>
            /// <param name="e">The event</param>
            public void Handle(string e)
            {
                Handled = true;
            }

            public bool Handled { get; set; }
        }
    }
}
