namespace CollectionSet
{
    [Serializable]
    public class SimpleMonitor : IDisposable
    {
        #region Campos

        private int _cantidadOcupada;

        #endregion
        #region Propiedades

        public bool Ocupado => _cantidadOcupada > 0;

        #endregion
        #region Metodos

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                --_cantidadOcupada;
            }
        }

        public void Entrar()
        {
            ++_cantidadOcupada;
        }

        #endregion
        #region Miembros IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
