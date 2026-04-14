using Azure.Data.Tables;
using MvcCoreAzureStorage.Models;

namespace MvcCoreAzureStorage.Services
{
    public class ServiceStorageTables
    {
        private TableClient tableClient;

        public ServiceStorageTables(TableServiceClient tableService)
        {
            this.tableClient = tableService.GetTableClient("clientes");
        }

        public async Task CreateClientAsync(int id, string nombre, string empresa, int edad, int salario)
        {
            Cliente cliente = new Cliente()
            {
                IdCliente = id,
                Nombre = nombre,
                Empresa = empresa,
                Edad = edad,
                Salario = salario
            };
            await this.tableClient.AddEntityAsync(cliente);
        }

        //LAS ENTIDADE DE TABLA SI DESEAMOS BUSCAR POR SU ID
        //SOLAMENTE NO PODEMOS, DEBEMOS HACERLO MEDIANTE
        //UNA BUSQUEDA POR PARTITION KEY Y ROW KEY
        public async Task<Cliente> FindClienteAsync(string partitionKey, string rowKey)
        {
            Cliente cliente = await this.tableClient.GetEntityAsync<Cliente>(partitionKey, rowKey);
            return cliente;
        }

        public async Task DeleteClienteAsync(string partitionKey, string rowKey)
        {
            await this.tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public async Task<List<Cliente>> GetClientesAsync()
        {
            List<Cliente> clientes = new List<Cliente>();
            //PARA LAS BUSQUEDAS SE UTILIZAN QUERY Y FILTER
            //AUNQUE NO BUSQUEMOS, SI QUEREMOS TODO, LE MANDAMOS
            //UN FILTER VACIO
            var query = this.tableClient.QueryAsync<Cliente>(filter: "");
            //EXTRAEMOS LOS DATOS DE LA CONSULTA DEL QUERY
            await foreach (var item in query)
            {
                clientes.Add(item);
            }
            return clientes;
        }

        public async Task<List<Cliente>> GetClientesEmpresaAsync(string empresa)
        {
            //TENEMOS DOS TIPOS DE FILTER, LOS DOS CON QUERY
            //1) SI UTILIZAMOS QueryAsync DEBEMOS ESCRIBIR UNA SINTAXIS ESPECIAL DENTRO DEL FILTER
            //string filter = $"Empresa eq '{empresa}'"; IGUAL
            //string filter = $"Empresa gt '{empresa}'"; MAYOR
            //string filter = $"Empresa lt '{empresa}'"; MENOR
            //string filter = $"Empresa eq '{empresa}' and Edad gt 30"; IGUAL Y MAYOR
            //string filter = $"Empresa eq '{empresa}' or Edad gt 30"; IGUAL O MAYOR
            //string filter = $"Empresa eq '{empresa}' and (Edad gt 30 or Salario gt 50000)"; IGUAL Y (MAYOR O MAYOR)

            //2) UTILIZAR Query PERMITE CONSULTAR OCN Lambda
            //PERO SE PIERDE EL ASINCRONO
            //Y NOS DEVUELVE TODO DIRECTAMENTE, NO DEBEMOS HACER UN BUCLE PARA EXTRAER LOS DATOS
            var query = this.tableClient.Query<Cliente>(c => c.Empresa == empresa);
            return query.ToList();
        }
    }
}
