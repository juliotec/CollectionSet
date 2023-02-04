using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace CollectionSet
{    
    [Serializable]
    public class ColeccionSet<T> : IList<T>, IList, ICollection<T>, ICollection, IEnumerable<T>, IEnumerable, IBindingListView, IBindingList, ICancelAddNew, IRaiseItemChangedEvents, INotifyCollectionChanged, IQueryable<T>, IQueryable, IOrderedQueryable<T>, IOrderedQueryable
    {
        #region Campos

        private readonly SimpleMonitor _monitorColeccionCambiada;
        private readonly IExpressionSerializer _iExpressionSerializer;
        private int _indiceAgregarNuevo;
        private bool _estaFiltrado;
        private IList<T> _elementosOriginales;
        [NonSerialized]
        private PropertyDescriptorCollection? _itemTypeProperties;
        [NonSerialized]
        private int _ultimoIndiceCambiado;
        [NonSerialized]
        private PropertyChangedEventHandler? _propertyChanged;
        private Expression<Func<T, bool>>? _filtroExpression;
                                                                                        
        #endregion
        #region Constructores
                                            
        public ColeccionSet()
        {
            IQueryable iQueryable;
                                            
            _elementosOriginales = new List<T>();
            _elementos = _elementosOriginales;
            iQueryable = _elementosOriginales.AsQueryable();
            _provider = iQueryable.Provider;
            _expression = iQueryable.Expression;
            _monitorColeccionCambiada = new SimpleMonitor();
            _iExpressionSerializer = new ExpressionSerializer();
            _elementType = typeof(T);
            InicializarConstructor();
        }
                                                                                                        
        public ColeccionSet(IEnumerable<T> iEnumerable)
        {
            if (iEnumerable is not IQueryable<T> iQueryable)
            {
                iQueryable = iEnumerable.AsQueryable();
            }

            if (iEnumerable is not IList<T> list)
            {
                list = new List<T>(iEnumerable);
            }           
       
            _elementos = _elementosOriginales = list;
            _provider = iQueryable.Provider;
            _expression = iQueryable.Expression;
            _monitorColeccionCambiada = new SimpleMonitor();
            _iExpressionSerializer = new ExpressionSerializer();
            _sortDescriptions = new ListSortDescriptionCollection();
            _elementType = typeof(T);
            InicializarConstructor();
        }

        #endregion
        #region Eventos

        public event ListChangedEventHandler ListChanged
        {
            add
            {
                var listChanged = _listChanged;
                ListChangedEventHandler? handler;

                do
                {
                    ListChangedEventHandler? handler2;
                                                                                    
                    handler = listChanged;
                    handler2 = (ListChangedEventHandler?)Delegate.Combine(handler, value);
                    listChanged = Interlocked.CompareExchange(ref _listChanged, handler2, handler);
                }
                while (listChanged != handler);
            }
            remove
            {
                var listChanged = _listChanged;
                ListChangedEventHandler? handler;

                do
                {
                    ListChangedEventHandler? handler2;
                                                                                    
                    handler = listChanged;
                    handler2 = (ListChangedEventHandler?)Delegate.Remove(handler, value);
                    listChanged = Interlocked.CompareExchange(ref _listChanged, handler2, handler);
                }
                while (listChanged != handler);
            }
        }
        [NonSerialized]
        private ListChangedEventHandler? _listChanged;
                                                                                    
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                var collectionChanged = _collectionChanged;
                NotifyCollectionChangedEventHandler? handler;

                do
                {
                    NotifyCollectionChangedEventHandler? handler2;
                                                                                    
                    handler = collectionChanged;
                    handler2 = (NotifyCollectionChangedEventHandler?)Delegate.Combine(handler, value);
                    collectionChanged = Interlocked.CompareExchange(ref _collectionChanged, handler2, handler);
                }
                while (collectionChanged != handler);
            }
            remove
            {
                var collectionChanged = _collectionChanged;
                NotifyCollectionChangedEventHandler? handler;

                do
                {
                    NotifyCollectionChangedEventHandler? handler2;
                                                                                    
                    handler = collectionChanged;
                    handler2 = (NotifyCollectionChangedEventHandler?)Delegate.Remove(handler, value);
                    collectionChanged = Interlocked.CompareExchange(ref _collectionChanged, handler2, handler);
                }
                while (collectionChanged != handler);
            }
        }
        [NonSerialized]
        private NotifyCollectionChangedEventHandler? _collectionChanged;

        public event AddingNewEventHandler AddingNew
        {
            add
            {
                var allowNewWasTrue = AllowNew;
                var addingNew = _addingNew;
                AddingNewEventHandler? handler;

                do
                {
                    AddingNewEventHandler? handler2;

                    handler = addingNew;
                    handler2 = (AddingNewEventHandler?)Delegate.Combine(handler, value);
                    addingNew = Interlocked.CompareExchange(ref _addingNew, handler2, handler);
                }
                while (addingNew != handler);

                if (allowNewWasTrue != AllowNew)
                {
                    ComprobarReentradaColeccionCambiada();
                    SobreColeccionReinicializada();
                }
            }
            remove
            {
                var allowNewWasTrue = AllowNew;
                var addingNew = _addingNew;
                AddingNewEventHandler? handler;

                do
                {
                    AddingNewEventHandler? handler2;

                    handler = addingNew;
                    handler2 = (AddingNewEventHandler?)Delegate.Remove(handler, value);
                    addingNew = Interlocked.CompareExchange(ref _addingNew, handler2, handler);
                }
                while (addingNew != handler);

                if (allowNewWasTrue != AllowNew)
                {
                    ComprobarReentradaColeccionCambiada();
                    SobreColeccionReinicializada();
                }
            }
        }
        [NonSerialized]
        private AddingNewEventHandler? _addingNew;

        #endregion
        #region Propiedades

        private static bool TypeTTieneConstructorPredeterminado
        {
            get
            {
                var typeT = typeof(T);
                                                                                                        
                return typeT.IsPrimitive || (typeT.GetConstructor(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null) != null);
            }
        }

        protected IList<T> Elementos => _elementos;
        private IList<T> _elementos;
                                                                                                        
        public bool LanzarEventosListaCambiada
        {
            get
            {
                return _lanzarEventosListaCambiada;
            }
            set
            {
                if (_lanzarEventosListaCambiada != value)
                {
                    _lanzarEventosListaCambiada = value;
                }
            }
        }
        private bool _lanzarEventosListaCambiada;
                                                                                                        
        public bool AllowEdit
        {
            get
            {
                return _allowEdit;
            }
            set
            {
                if (_allowEdit != value)
                {
                    ComprobarReentradaColeccionCambiada();
                    _allowEdit = value;
                    SobreColeccionReinicializada();
                }
            }
        }
        private bool _allowEdit;
                                                                                                        
        public bool AllowNew
        {
            get
            {
                return _allowNew;
            }
            set
            {
                if (_elementos.IsReadOnly || !TypeTTieneConstructorPredeterminado)
                {
                    throw new NotSupportedException();
                }
                                                                        
                if (_allowNew != value)
                {
                    ComprobarReentradaColeccionCambiada();
                    _allowNew = value;
                    SobreColeccionReinicializada();
                }
            }
        }
        private bool _allowNew;
                                                                                                        
        public bool AllowRemove
        {
            get
            {
                return _allowRemove;
            }
            set
            {
                if (_elementos.IsReadOnly)
                {
                    throw new NotSupportedException();
                }
                                                                        
                if (_allowRemove != value)
                {
                    ComprobarReentradaColeccionCambiada();
                    _allowRemove = value;
                    SobreColeccionReinicializada();
                }
            }
        }
        private bool _allowRemove;

        public bool IsSorted => _isSorted;
        private bool _isSorted;
                                                                                                        
        public T this[int indice]
        {
            get
            {
                return _elementos[indice];
            }
            set
            {
                if (_elementos.IsReadOnly)
                {
                    throw new NotSupportedException();
                }
                                                                                                        
                if ((indice < 0) || (indice >= _elementos.Count))
                {
                    throw new ArgumentOutOfRangeException(nameof(indice));
                }
                                                                                                        
                ComprobarReentradaColeccionCambiada();
                                                                                                        
                var itemAnterior = _elementos[indice];
                                                                                                        
                if (_raisesItemChangedEvents)
                {
                    DesanclarPropiedadCambiada(itemAnterior);
                }
                                                                                                        
                _elementos[indice] = value;
                                            
                if (_elementos != _elementosOriginales)
                {
                    var indiceOriginales = _elementosOriginales.IndexOf(itemAnterior);
                                            
                    _elementosOriginales[indiceOriginales] = value;
                }                
                                                                                                        
                if (_raisesItemChangedEvents)
                {
                    AnclarPropiedadCambiada(value);
                }
                                                                                                        
                if (_lanzarEventosListaCambiada)
                {
                    SobreListaCambiada(new ListChangedEventArgs(ListChangedType.ItemChanged, indice));
                }
                                                                                                        
                SobreColeccionCambiada(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, itemAnterior, indice));
            }
        }

        public bool RaisesItemChangedEvents => _raisesItemChangedEvents;
        private bool _raisesItemChangedEvents;
                                                                                                        
        public bool IsFixedSize
        {
            get
            {
                if (_elementos is IList iList)
                {
                    return iList.IsFixedSize;
                }

                return _elementos.IsReadOnly;
            }
        }

        public bool IsReadOnly => _elementos.IsReadOnly;

        public int Count => _elementos.Count;

        public object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    if (_elementos is ICollection iCollection)
                    {
                        _syncRoot = iCollection.SyncRoot;
                    }
                    else
                    {
                        Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                    }
                }
                                                                                                        
                return _syncRoot;
            }
        }
        [NonSerialized]
        private object? _syncRoot;
                                                                                                        
        public string? Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (_filter != value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        RemoveFilter();
                    }
                    else
                    {
                        ComprobarReentradaColeccionCambiada();
                        EndNew(_indiceAgregarNuevo);
                        _filtroExpression = _iExpressionSerializer.ToExpression<Expression<Func<T, bool>>>(XElement.Parse(value));
                        
                        if (_filtroExpression != null)
                        {
                            var iQueryable = _elementosOriginales.AsQueryable().Where(_filtroExpression);

                            if (_isSorted && _sortDescriptions != null)
                            {
                                Ejecutar(iQueryable.OrderBy(_sortDescriptions), false);
                            }
                            else if (_isSorted && _sortProperty != null)
                            {
                                Ejecutar(iQueryable.OrderBy(_sortProperty, _sortDirection), false);
                            }
                            else
                            {
                                Ejecutar(iQueryable, false);
                            }
                        }
                                            
                        _filter = value;
                        _estaFiltrado = true;
                        SobreColeccionReinicializada();
                    }
                }
            }
        }
        private string? _filter;

        public ListSortDirection SortDirection => _sortDirection;
        private ListSortDirection _sortDirection;

        public PropertyDescriptor? SortProperty => _sortProperty;
        [NonSerialized]
        private PropertyDescriptor? _sortProperty;
                                                                                                        
        public ListSortDescriptionCollection SortDescriptions
        {
            get
            {
                if (_sortDescriptions == null)
                {
                    throw new ArgumentNullException(nameof(SortDescriptions));
                }

                return _sortDescriptions;
            }
        }
        [NonSerialized]
        private ListSortDescriptionCollection? _sortDescriptions;

        public bool IsSynchronized => false;

        public bool SupportsChangeNotification => true;

        public bool SupportsSearching => true;

        public bool SupportsSorting => true;

        public bool SupportsAdvancedSorting => true;

        public bool SupportsFiltering => true;

        public IQueryProvider Provider => _provider;
        private IQueryProvider _provider;

        public Type ElementType => _elementType;
        private readonly Type _elementType;

        public Expression Expression => _expression;
        private Expression _expression;

        #endregion
        #region Metodos

        private static bool EsObjetoCompatible(object? valor) => (valor is T) || ((valor == null) && (default(T) == null));

        private void ValoresPorDefecto()
        {
            _filter = string.Empty;
            _filtroExpression = null;
            _estaFiltrado = false;
            _sortDirection = ListSortDirection.Ascending;
            _sortProperty = null;
            _sortDescriptions = null;
            _isSorted = false;
        }
                                                                                                       
        private void InicializarConstructor()
        {
            _indiceAgregarNuevo = -1;
            _lanzarEventosListaCambiada = true;
            _ultimoIndiceCambiado = -1;
            _allowEdit = true;
            _allowRemove = !_elementos.IsReadOnly;
            _allowNew = !_elementos.IsReadOnly && TypeTTieneConstructorPredeterminado;
            ValoresPorDefecto();
                                                                
            if (typeof(INotifyPropertyChanged).IsAssignableFrom(_elementType))
            {
                _raisesItemChangedEvents = true;
                                                                                                        
                for (var i = 0; i < _elementos.Count; i++)
                {
                    AnclarPropiedadCambiada(_elementos[i]);
                }
            }            
        }
                                                                                                        
        private void PropiedadCambiadaHija(object? sender, PropertyChangedEventArgs e)
        { 
            if (!LanzarEventosListaCambiada)
            {
                return;
            }
                                                                                                            
            if (sender == null || e == null || string.IsNullOrEmpty(e.PropertyName))
            {
                ComprobarReentradaColeccionCambiada();
                SobreColeccionReinicializada();
                return;
            }
                                                                                            
            T elemento;
                                                                                                            
            try
            {
                elemento = (T)sender;
            }
            catch (InvalidCastException)
            {
                ComprobarReentradaColeccionCambiada();
                SobreColeccionReinicializada();
                return;
            }
                                                                                                            
            var ultimoIndiceCambiado2 = _ultimoIndiceCambiado;
            var item = this[ultimoIndiceCambiado2];

            if (ultimoIndiceCambiado2 < 0 || ultimoIndiceCambiado2 >= Count || (item != null && !item.Equals(elemento)))
            {
                ultimoIndiceCambiado2 = IndexOf(elemento);
                _ultimoIndiceCambiado = ultimoIndiceCambiado2;
            }

            ComprobarReentradaColeccionCambiada();

            if (ultimoIndiceCambiado2 == -1)
            {                
                DesanclarPropiedadCambiada(elemento);                
                SobreColeccionReinicializada();
            }
            else
            {
                _itemTypeProperties ??= TypeDescriptor.GetProperties(_elementType);                                                                                                            
                SobreListaCambiada(new ListChangedEventArgs(ListChangedType.ItemChanged, ultimoIndiceCambiado2, _itemTypeProperties.Find(e.PropertyName, true)));
                SobreColeccionCambiada(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
                                                                                                        
        private void DesanclarPropiedadCambiada(T elemento)
        {
            if ((elemento is INotifyPropertyChanged iNotifyPropertyChanged) && (_propertyChanged != null))
            {
                iNotifyPropertyChanged.PropertyChanged -= _propertyChanged;
            }
        }
                                                                                                        
        private void AnclarPropiedadCambiada(T elemento)
        {
            if (elemento is INotifyPropertyChanged iNotifyPropertyChanged)
            {
                _propertyChanged ??= new PropertyChangedEventHandler(PropiedadCambiadaHija);
                iNotifyPropertyChanged.PropertyChanged += _propertyChanged;
            }
        }
                                                                                   
        private void SobreColeccionReinicializada()
        {
            if (_lanzarEventosListaCambiada)
            {
                SobreListaCambiada(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
                                            
            SobreColeccionCambiada(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
                                                        
        private void Ejecutar(IEnumerable<T> iEnumerable, bool originales)
        {
            if (iEnumerable is not IList<T> lista)
            {
                lista = iEnumerable.ToList();
            }

            if (iEnumerable is not IQueryable<T> iQueryable)
            {
                iQueryable = lista.AsQueryable();
            }
                                                
            if (originales)
            {
                if (_raisesItemChangedEvents)
                {
                    for (var i = 0; i < _elementosOriginales.Count; i++)
                    {
                        DesanclarPropiedadCambiada(_elementosOriginales[i]);
                    }
                }
                                                
                _elementosOriginales = lista;
                _elementos = lista;
                                    
                if (_raisesItemChangedEvents)
                {
                    for (var i = 0; i < _elementosOriginales.Count; i++)
                    {
                        AnclarPropiedadCambiada(_elementosOriginales[i]);
                    }
                }
            }
            else
            {
                _elementos = lista;
            }
                                            
            _provider = iQueryable.Provider;
            _expression = iQueryable.Expression;
        }
    
        protected IDisposable BloquearReentradaColeccionCambiada()
        {
            _monitorColeccionCambiada.Entrar();
    
            return _monitorColeccionCambiada;
        }
    
        protected void ComprobarReentradaColeccionCambiada()
        {
            if (_monitorColeccionCambiada.Ocupado && (_collectionChanged != null) && (_collectionChanged.GetInvocationList().Length > 1))
            {
                throw new InvalidOperationException();
            }
        }

        protected virtual void SobreListaCambiada(ListChangedEventArgs e) => _listChanged?.Invoke(this, e);

        protected virtual void SobreColeccionCambiada(NotifyCollectionChangedEventArgs e)
        {
            if (_collectionChanged != null)
            {
                using (BloquearReentradaColeccionCambiada())
                {
                    _collectionChanged(this, e);
                }
            }
        }

        protected virtual void SobreAgregandoNuevo(AddingNewEventArgs e) => _addingNew?.Invoke(this, e);

        public IEnumerator<T> GetEnumerator() => _elementos.GetEnumerator();

        public void Llenar(IEnumerable<T> iEnumerable)
        {
            if (iEnumerable == null)
            {
                throw new ArgumentNullException(nameof(iEnumerable));
            }
                                                                                    
            if (!_allowNew || (!_allowRemove && _elementos.Count > 0))
            {
                throw new NotSupportedException();
            }                                    
                                       
            ComprobarReentradaColeccionCambiada();
            EndNew(_indiceAgregarNuevo);
            Ejecutar(iEnumerable, true);
            ValoresPorDefecto();
            SobreColeccionReinicializada();                        
        }
                    
        public void Mover(int indiceAnterior, int indiceNuevo)
        {
            ComprobarReentradaColeccionCambiada();
            EndNew(_indiceAgregarNuevo);
                                                                
            var elemento = _elementos[indiceAnterior];
                                                                
            _elementos.RemoveAt(indiceAnterior);
            _elementos.Insert(indiceNuevo, elemento);
                                                                
            if (_lanzarEventosListaCambiada)
            {
                SobreListaCambiada(new ListChangedEventArgs(ListChangedType.ItemMoved, indiceNuevo, indiceAnterior));
            }
                                                                
            SobreColeccionCambiada(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, elemento, indiceNuevo, indiceAnterior));
        }
                                                                
        public T? AddNew()
        {
            var e = new AddingNewEventArgs(null);

            SobreAgregandoNuevo(e);

            if (e.NewObject == null)
            {
                if (TypeTTieneConstructorPredeterminado)
                {
                    e.NewObject = Activator.CreateInstance<T>();
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            _indiceAgregarNuevo = Add(e.NewObject);

            return (T?)e.NewObject;
        }
                                                                                                        
        public void CancelNew(int indice)
        {
            if ((_indiceAgregarNuevo >= 0) && (_indiceAgregarNuevo == indice))
            {
                RemoveAt(_indiceAgregarNuevo);
                _indiceAgregarNuevo = -1;
            }
        }
                                                                                                        
        public void EndNew(int indice)
        {
            if ((_indiceAgregarNuevo >= 0) && (_indiceAgregarNuevo == indice))
            {
                _indiceAgregarNuevo = -1;
            }
        }
                                                                                                        
        public void RemoveFilter()
        {
            if (_estaFiltrado)
            {
                ComprobarReentradaColeccionCambiada();
                EndNew(_indiceAgregarNuevo);
                                            
                if (_isSorted && _sortDescriptions != null)
                {
                    Ejecutar(_elementosOriginales.AsQueryable().OrderBy(_sortDescriptions), false);
                }
                else if (_isSorted && _sortProperty != null)
                {
                    Ejecutar(_elementosOriginales.AsQueryable().OrderBy(_sortProperty, _sortDirection), false);
                }
                else
                {
                    _elementos = _elementosOriginales;
                }
                                            
                _filter = string.Empty;
                _estaFiltrado = false;              
                SobreColeccionReinicializada();
            }
        }
                                                                                                        
        public void RemoveSort()
        {
            if (_isSorted)
            {
                ComprobarReentradaColeccionCambiada();
                EndNew(_indiceAgregarNuevo);
                                                                
                if (_estaFiltrado && _filtroExpression != null)
                {
                    Ejecutar(_elementosOriginales.AsQueryable().Where(_filtroExpression), false);
                }
                else
                {
                    _elementos = _elementosOriginales;
                }
                                                                                                        
                _sortDirection = ListSortDirection.Ascending;
                _sortProperty = null;
                _sortDescriptions = null;
                _isSorted = false;
                SobreColeccionReinicializada();
            }
        }
                                                                                        
        public void ApplySort(PropertyDescriptor propertyDescriptor, ListSortDirection listSortDirection)
        {
            if (propertyDescriptor == null)
            {
                throw new ArgumentNullException(nameof(propertyDescriptor));
            }
                                                                
            if (_elementType != propertyDescriptor.ComponentType)
            {
                throw new ArgumentOutOfRangeException(nameof(propertyDescriptor));
            }            
                                                                
            ComprobarReentradaColeccionCambiada();
            EndNew(_indiceAgregarNuevo);
                                            
            IQueryable<T> iQueriable;
                                            
            if (_estaFiltrado && _filtroExpression != null)
            {
                iQueriable = _elementosOriginales.AsQueryable().Where(_filtroExpression);
            }
            else
            {
                iQueriable = _elementos.AsQueryable();
            }

            Ejecutar(iQueriable.OrderBy(propertyDescriptor, listSortDirection), false);
            _sortProperty = propertyDescriptor;
            _sortDirection = listSortDirection;
            _sortDescriptions = null;
            _isSorted = true;
            SobreColeccionReinicializada();
        }
                                                                                        
        public void ApplySort(ListSortDescriptionCollection? listSortDescriptionCollection)
        {
            if (listSortDescriptionCollection == null)
            {
                throw new ArgumentNullException(nameof(listSortDescriptionCollection));
            }
                                                                                        
            if (listSortDescriptionCollection.Count > 0)
            {
                ComprobarReentradaColeccionCambiada();
                EndNew(_indiceAgregarNuevo);
                                            
                IQueryable<T> iQueriable;
                                            
                if (_estaFiltrado && _filtroExpression != null)
                {
                    iQueriable = _elementosOriginales.AsQueryable().Where(_filtroExpression);
                }
                else
                {
                    iQueriable = _elementos.AsQueryable();
                }
                    
                Ejecutar(iQueriable.OrderBy(listSortDescriptionCollection), false);

                var listSortDescription = listSortDescriptionCollection[0];

                if (listSortDescription == null)
                {
                    _sortProperty = null;
                    _sortDirection = ListSortDirection.Ascending;
                }
                else
                {
                    _sortProperty = listSortDescription.PropertyDescriptor;
                    _sortDirection = listSortDescription.SortDirection;
                }

                _sortDescriptions = listSortDescriptionCollection;
                _isSorted = true;
                SobreColeccionReinicializada();
            }
        }
                                                                                        
        public int Find(PropertyDescriptor descriptorPropiedad, object valor)
        {
            if (descriptorPropiedad == null)
            {
                throw new ArgumentNullException(nameof(descriptorPropiedad));
            }
                                                                                    
            for (var i = 0; i < _elementos.Count; i++)
            {
                var valor2 = descriptorPropiedad.GetValue(_elementos[i]);
                                                                                        
                if (valor2 != null && valor2.Equals(valor))
                {
                    return i;
                }
            }
                                                                                        
            return -1;
        }
                                                                                          
        public bool Insert(int indice, T? elemento)
        {
            if (!_allowNew || (indice < 0) || (indice > _elementos.Count) || elemento == null || _elementosOriginales.Contains(elemento))
            {
                return false;
            }
                                                                                        
            ComprobarReentradaColeccionCambiada();
            EndNew(_indiceAgregarNuevo);
            _elementos.Insert(indice, elemento);
                                            
            if (_elementos != _elementosOriginales)
            {
                _elementosOriginales.Insert(indice, elemento);
            }
                                                                                                        
            if (_raisesItemChangedEvents)
            {
                AnclarPropiedadCambiada(elemento);
            }
                                                                                                        
            if (_lanzarEventosListaCambiada)
            {
                SobreListaCambiada(new ListChangedEventArgs(ListChangedType.ItemAdded, indice));
            }
                                                                                                        
            SobreColeccionCambiada(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, elemento, indice));
                                                                                                        
            return true;
        }

        public bool Insert(int index, object? valor) => EsObjetoCompatible(valor) && Insert(index, (T?)valor);

        public int Add(T elemento) => Insert(_elementos.Count, elemento) ? (_elementos.Count - 1) : -1;

        public int Add(object? valor) => Insert(_elementos.Count, valor) ? (_elementos.Count - 1) : -1;

        public bool RemoveAt(int indice)
        {
            if ((!_allowRemove && ((_indiceAgregarNuevo < 0) || (_indiceAgregarNuevo != indice))) || (indice < 0) || (indice >= _elementos.Count))
            {
                return false;
            }
                                                                                        
            ComprobarReentradaColeccionCambiada();
            EndNew(_indiceAgregarNuevo);
                                                                                        
            var elemento = _elementos[indice];
                                                                                                        
            if (_raisesItemChangedEvents)
            {
                DesanclarPropiedadCambiada(elemento);
            }
                                                                                                        
            _elementos.RemoveAt(indice);
                                            
            if (_elementos != _elementosOriginales)
            {
                _elementosOriginales.Remove(elemento);
            }
                                                                                                        
            if (_lanzarEventosListaCambiada)
            {
                SobreListaCambiada(new ListChangedEventArgs(ListChangedType.ItemDeleted, indice));
            }
                                                                                                        
            SobreColeccionCambiada(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, elemento, indice));
                                                                                                        
            return true;
        }

        public bool Remove(T? elemento) => elemento != null && RemoveAt(_elementos.IndexOf(elemento));

        public bool Remove(object? value) => EsObjetoCompatible(value) && Remove((T?)value);

        public void Clear()
        {
            if (!_allowRemove)
            {
                throw new NotSupportedException();
            }
                                                                        
            ComprobarReentradaColeccionCambiada();
            EndNew(_indiceAgregarNuevo);
                                                                        
            if (_raisesItemChangedEvents)
            {
                for (var i = 0; i < _elementosOriginales.Count; i++)
                {
                    DesanclarPropiedadCambiada(_elementosOriginales[i]);
                }
            }
                                                                        
            _elementos.Clear();
            _elementosOriginales.Clear();
    
            var iQueryable = _elementosOriginales.AsQueryable();
    
            _provider = iQueryable.Provider;
            _expression = iQueryable.Expression;
    		_elementos = _elementosOriginales;
            ValoresPorDefecto();
            SobreColeccionReinicializada();
        }

        public bool Contains(T? elemento) => elemento != null && _elementos.Contains(elemento);

        public bool Contains(object? valor) => EsObjetoCompatible(valor) && Contains((T?)valor);

        public int IndexOf(T? elemento) => elemento != null ? _elementos.IndexOf(elemento) : -1;

        public int IndexOf(object? valor) => EsObjetoCompatible(valor) ? IndexOf((T?)valor) : -1;

        public void AddIndex(PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor == null)
            {
                throw new ArgumentNullException(nameof(propertyDescriptor));
            }
                                                                                                
            throw new NotSupportedException();
        }
                                                                                                
        public void RemoveIndex(PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor == null)
            {
                throw new ArgumentNullException(nameof(propertyDescriptor));
            }
                                                                                                
            throw new NotSupportedException();
        }

        public void CopyTo(T[] elementos, int indice) => _elementos.CopyTo(elementos, indice);

        public void CopyTo(Array array, int indice)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
                                                                                                        
            if (array.Rank != 1 || array.GetLowerBound(0) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(array));
            }
                                                                                                        
            if (indice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indice));
            }
                                                                                                        
            if ((array.Length - indice) < Count)
            {
                throw new ArgumentOutOfRangeException(nameof(array));
            }

            if (array is T[] localArray)
            {
                CopyTo(localArray, indice);
            }
            else
            {
                var elementType = array.GetType().GetElementType();

                if ((elementType != null && !elementType.IsAssignableFrom(_elementType) && !_elementType.IsAssignableFrom(elementType)) || array is not object?[] objectoArreglo)
                {
                    throw new ArgumentOutOfRangeException(nameof(array));
                }

                try
                {
                    for (var i = 0; i < _elementos.Count; i++)
                    {
                        objectoArreglo[indice++] = _elementos[i];
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentOutOfRangeException(nameof(array));
                }
            }
        }

        #endregion
        #region Miembros IList<T>

        int IList<T>.IndexOf(T elemento) => IndexOf(elemento);

        void IList<T>.Insert(int indice, T elemento) => Insert(indice, elemento);

        void IList<T>.RemoveAt(int indice) => RemoveAt(indice);

        T IList<T>.this[int indice]
        {
            get
            {
                return this[indice];
            }
            set
            {
                this[indice] = value;
            }
        }

        #endregion
        #region Miembros IList

        int IList.Add(object? valor) => Add(valor);

        void IList.Clear() => Clear();

        bool IList.Contains(object? valor) => Contains(valor);

        int IList.IndexOf(object? valor) => IndexOf(valor);

        void IList.Insert(int indice, object? valor) => Insert(indice, valor);

        void IList.Remove(object? valor) => Remove(valor);

        void IList.RemoveAt(int indice) => RemoveAt(indice);

        bool IList.IsFixedSize => IsFixedSize;

        bool IList.IsReadOnly => IsReadOnly;

        object? IList.this[int indice]
        {
            get
            {
                return _elementos[indice];
            }
            set
            {
                if (!EsObjetoCompatible(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                                                                        
                this[indice] = (T)value;
            }
        }

        #endregion
        #region Miembros ICollection<T>

        void ICollection<T>.Add(T elemento) => Add(elemento);

        void ICollection<T>.Clear() => Clear();

        bool ICollection<T>.Contains(T elemento) => Contains(elemento);

        void ICollection<T>.CopyTo(T[] elementos, int indice) => CopyTo(elementos, indice);

        bool ICollection<T>.Remove(T elemento) => Remove(elemento);

        int ICollection<T>.Count => Count;

        bool ICollection<T>.IsReadOnly => IsReadOnly;

        #endregion
        #region Miembros ICollection

        void ICollection.CopyTo(Array array, int indice) => CopyTo(array, indice);

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => IsSynchronized;

        object ICollection.SyncRoot => SyncRoot;

        #endregion
        #region Miembros IEnumerable<T>

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        #endregion
        #region Miembros IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
        #region Miembros IBindingListView

        string? IBindingListView.Filter
        {
            get
            {
                return Filter;
            }
            set
            {
                Filter = value;
            }
        }

        ListSortDescriptionCollection IBindingListView.SortDescriptions => SortDescriptions;

        bool IBindingListView.SupportsAdvancedSorting => SupportsAdvancedSorting;

        bool IBindingListView.SupportsFiltering => SupportsFiltering;

        void IBindingListView.ApplySort(ListSortDescriptionCollection listSortDescriptionCollection) => ApplySort(listSortDescriptionCollection);

        void IBindingListView.RemoveFilter() => RemoveFilter();

        #endregion
        #region Miembros IBindingList

        event ListChangedEventHandler IBindingList.ListChanged
        {
            add
            {
                ListChanged += value;
            }
            remove
            {
                ListChanged -= value;
            }
        }

        bool IBindingList.AllowEdit => AllowEdit;

        bool IBindingList.AllowNew => AllowNew;

        bool IBindingList.AllowRemove => AllowRemove;

        bool IBindingList.IsSorted => IsSorted;

        ListSortDirection IBindingList.SortDirection => SortDirection;

        PropertyDescriptor? IBindingList.SortProperty => SortProperty;

        bool IBindingList.SupportsChangeNotification => SupportsChangeNotification;

        bool IBindingList.SupportsSearching => SupportsSearching;

        bool IBindingList.SupportsSorting => SupportsSorting;

        void IBindingList.AddIndex(PropertyDescriptor propertyDescriptor) => AddIndex(propertyDescriptor);

        object? IBindingList.AddNew() => AddNew();

        void IBindingList.ApplySort(PropertyDescriptor propertyDescriptor, ListSortDirection listSortDirection) => ApplySort(propertyDescriptor, listSortDirection);

        int IBindingList.Find(PropertyDescriptor propertyDescriptor, object valor) => Find(propertyDescriptor, valor);

        void IBindingList.RemoveIndex(PropertyDescriptor propertyDescriptor) => RemoveIndex(propertyDescriptor);

        void IBindingList.RemoveSort() => RemoveSort();

        #endregion
        #region Miembros ICancelAddNew

        void ICancelAddNew.CancelNew(int indice) => CancelNew(indice);

        void ICancelAddNew.EndNew(int indice) => EndNew(indice);

        #endregion
        #region Miembros IRaiseItemChangedEvents

        bool IRaiseItemChangedEvents.RaisesItemChangedEvents => RaisesItemChangedEvents;

        #endregion
        #region Miembros INotifyCollectionChanged

        event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                CollectionChanged += value;
            }
            remove
            {
                CollectionChanged -= value;
            }
        }

        #endregion
        #region Miembros IQueryable

        IQueryProvider IQueryable.Provider => Provider;

        Type IQueryable.ElementType => ElementType;

        Expression IQueryable.Expression => Expression;

        #endregion
    }
}
