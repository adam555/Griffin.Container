using System;
using System.Collections.Generic;
using Griffin.Container.BuildPlans;

namespace Griffin.Container
{
    /// <summary>
    /// Main container implementation.
    /// </summary>
    /// <remarks>Registrations should be managed by a <see cref="IContainerRegistrar"/> implementation and this
    /// container should the be built by a <see cref="IContainerBuilder"/> implementation. Look at the namespace documentation
    /// for an example.</remarks>
    public class Container : ContainerBase, IParentContainer
    {
        private readonly IInstanceStorageFactory _factory;
        private readonly IInstanceStorage _storage;

        [ThreadStatic]
        static private IChildContainer _childContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="serviceMappings">The service mappings which as been generated by a <see cref="IContainerBuilder"/>.</param>
        public Container(IServiceMappings serviceMappings)
            : base(serviceMappings)
        {
            if (serviceMappings == null) throw new ArgumentNullException("serviceMappings");

            _factory = new DefaultInstanceStorageFactory();
            _storage = _factory.CreateParent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="serviceMappings">The service mappings which as been generated by a <see cref="IContainerBuilder"/>.</param>
        /// <param name="factory">Used to create the storage</param>
        public Container(IServiceMappings serviceMappings, IInstanceStorageFactory factory)
            : base(serviceMappings)
        {
            if (serviceMappings == null) throw new ArgumentNullException("serviceMappings");
            _factory = factory;
            _storage = factory.CreateParent();
        }

        /// <summary>
        /// Gets current child container (if any).
        /// </summary>
        internal static IChildContainer ChildContainer
        {
            get { return _childContainer; }
        }

        #region IParentContainer Members

        /// <summary>
        /// Creates the child container.
        /// </summary>
        /// <returns>Created container.</returns>
        public virtual IChildContainer CreateChildContainer()
        {
            var prevContainer = _childContainer;
            _childContainer = new ChildContainer(ServiceMappings, _storage, _factory.CreateScoped(),
                                 () => _childContainer = prevContainer);
            return _childContainer;
        }

        /// <summary>
        /// Gets current child
        /// </summary>
        /// <remarks>Returns the last one created (in the current thread) if several child containers have been created.</remarks>
        public IChildContainer CurrentChild
        {
            get { return _childContainer; }
        }

        #endregion

        /// <summary>
        /// Gets storage for scoped objects.
        /// </summary>
        protected override IInstanceStorage ChildStorage
        {
            get { return null; }
        }

        /// <summary>
        /// Gets storage for singletons
        /// </summary>
        protected override IInstanceStorage RootStorage
        {
            get { return _storage; }
        }
    }
}