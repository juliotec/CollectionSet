using System.Linq.Expressions;

namespace CollectionSet
{
    public partial class TestForm : Form
    {
        private readonly CollectionSet<TestEntity> _collectionSet;
        private readonly List<TestEntity> _normalList;

        public TestForm()
        {
             _collectionSet = new CollectionSet<TestEntity>()
            {
                new TestEntity()
                {
                    Id = 1,
                    Name= "Juan",
                    Description = "Esta en su casa",
                    Type = "Cansado"
                },
                new TestEntity()
                {
                    Id = 2,
                    Name= "Pedro",
                    Description = "No Esta",
                    Type = "Solo"
                },
                new TestEntity()
                {
                    Id = 3,
                    Name= "Ana",
                    Description = "Anda jugando",
                    Type = "Comiendo"
                }
            };
            _normalList = new List<TestEntity>()
            {
                new TestEntity()
                {
                    Id = 1,
                    Name= "JuanList",
                    Description = "Esta en su casa List",
                    Type = "Cansado List"
                },
                new TestEntity()
                {
                    Id = 2,
                    Name= "PedroList",
                    Description = "No Esta List",
                    Type = "Solo List"
                },
                new TestEntity()
                {
                    Id = 3,
                    Name= "AnaList",
                    Description = "Anda jugando List",
                    Type = "Comiendo List"
                }
            };
            InitializeComponent();
        }

        private void TestFormLoad(object sender, EventArgs e)
        {
            dataGridView.DataSource = _collectionSet;
        }

        private void CambiarListButtonClick(object sender, EventArgs e)
        {
            dataGridView.DataSource = _normalList;           
        }

        private void CambiarCollectionSetButtonClick(object sender, EventArgs e)
        {
            dataGridView.DataSource = _collectionSet;
        }

        private void AllowNewFalseButtonClick(object sender, EventArgs e)
        {
            _collectionSet.AllowNew = false;
        }

        private void AllowNewTrueButtonClick(object sender, EventArgs e)
        {
            _collectionSet.AllowNew = true;
        }

        private void AllowEditFalseButtonClick(object sender, EventArgs e)
        {
            _collectionSet.AllowEdit = false;
        }

        private void AllowEditTrueButtonClick(object sender, EventArgs e)
        {
            _collectionSet.AllowEdit = true;
        }

        private void FiltarTextBoxTextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(filtarTextBox.Text))
            {
                _collectionSet.FilterExpression = null;
            }
            else
            {
                _collectionSet.FilterExpression = (x) => !string.IsNullOrWhiteSpace(x.Name) && x.Name.StartsWith(filtarTextBox.Text);
            }
        }
    }
}