namespace CollectionSet
{
    public class NumericStringComparer : IComparer<string>
    {
        #region Propiedades

        public static NumericStringComparer Default => _default ??= new NumericStringComparer();
        private static NumericStringComparer? _default;

        #endregion
        #region Miembros IComparer<string>

        public int Compare(string? primero, string? segundo)
        {
            if (primero == segundo)
            {
                return 0;
            }

            if (primero == null)
            {
                return -1;
            }

            if (segundo == null)
            {
                return 1;
            }

            var primeroIsNumerico = int.TryParse(primero.Split(' ')[0], out int primeroNumerico);
            var segundoIsNumerico = int.TryParse(segundo.Split(' ')[0], out int segundoNumerico);

            return primeroIsNumerico ? (segundoIsNumerico ? primeroNumerico.CompareTo(segundoNumerico) : -1) : (segundoIsNumerico ? 1 : primero.CompareTo(segundo));
        }

        #endregion
    }
}
