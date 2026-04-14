using Azure;
using Azure.Data.Tables;

namespace MvcCoreAzureStorage.Models
{
    public class Cliente : ITableEntity
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int Salario { get; set; }

        //ID CLIENTE: ROW LEY
        //CUANDO EL USUARIO ALMACENE UN ID DE CLIENTE
        //NOSOTROS ALMACENAMOS ROWKEY
        private int _idCliente;
        public int IdCliente
        {
            get { return this._idCliente; }
            set
            {
                this._idCliente = value;
                this.RowKey = value.ToString();
            }
        }

        private string _Empresa;
        public string Empresa
        {
            get { return this._Empresa; }
            set
            {
                this._Empresa = value;
                this.PartitionKey = value.ToString();
            }
        }

        //EMPRESA: PARTITION KEY
        //CUANDO EL USUARIO ALMACENE UNA EMPRESA
        //ALMACENAMOS PARTITION KEY
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
