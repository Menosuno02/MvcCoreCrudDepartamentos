using MvcCoreCrudDepartamentos.Models;
using System.Data;
using System.Data.SqlClient;

#region PROCEDIMIENTOS ALMACENADOS
/*
CREATE PROCEDURE SP_INSERTDEPARTAMENTO
(@NOMBRE NVARCHAR(50), @LOCALIDAD NVARCHAR(50))
AS
    DECLARE @NEXTID INT
    SELECT @NEXTID = MAX(DEPT_NO) + 1
    FROM DEPT
    INSERT INTO DEPT
    VALUES(@NEXTID, @NOMBRE, @LOCALIDAD)
GO
*/
#endregion

namespace MvcCoreCrudDepartamentos.Repositories
{
    public class RepositoryDepartamentos
    {
        SqlConnection cn;
        SqlCommand com;
        SqlDataReader reader;

        public RepositoryDepartamentos()
        {
            string connectionString = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=HOSPITAL;User ID=sa;Password=MCSD2023;Encrypt=False";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
        }

        // Vamos a realizar las consultas sobre la BBDD
        // de forma asíncrona, con async/await
        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            string sql = "SELECT * FROM DEPT";
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();
            List<Departamento> departamentos = new List<Departamento>();
            while (await this.reader.ReadAsync())
            {
                Departamento dept = new Departamento();
                dept.IdDepartamento = int.Parse(this.reader["DEPT_NO"].ToString());
                dept.Nombre = this.reader["DNOMBRE"].ToString();
                dept.Localidad = this.reader["LOC"].ToString();
                departamentos.Add(dept);
            }
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            return departamentos;
        }

        public async Task<Departamento> FindDepartamentoAsync(int id)
        {
            string sql = "SELECT * FROM DEPT WHERE DEPT_NO = @ID";
            this.com.Parameters.AddWithValue("@ID", id);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();
            // Al buscar, debemos averiguar si existen datos o no
            // en el repository. Siempre que no existan, el repo
            // debe devolver NULL
            Departamento departamento = null;
            if (await this.reader.ReadAsync())
            {
                departamento = new Departamento();
                departamento.IdDepartamento = int.Parse(this.reader["DEPT_NO"].ToString());
                departamento.Nombre = this.reader["DNOMBRE"].ToString();
                departamento.Localidad = this.reader["LOC"].ToString();
            }
            else
            {

            }
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
            return departamento;
        }

        public async Task InsertDepartamentoAsync(string nombre, string localidad)
        {
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_INSERTDEPARTAMENTO";
            this.com.Parameters.AddWithValue("@NOMBRE", nombre);
            this.com.Parameters.AddWithValue("@LOCALIDAD", localidad);
            await this.cn.OpenAsync();
            int result = await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }

        public async Task UpdateDepartamentosAsync(int id, string nombre, string localidad)
        {
            string sql = "UPDATE DEPT SET DNOMBRE = @NOMBRE, LOC = @LOCALIDAD WHERE DEPT_NO = @ID";
            this.com.Parameters.AddWithValue("@NOMBRE", nombre);
            this.com.Parameters.AddWithValue("@LOCALIDAD", localidad);
            this.com.Parameters.AddWithValue("@ID", id);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            int result = await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }

        public async Task DeleteDepartamentoAsync(int id)
        {
            string sql = "DELETE FROM DEPT WHERE DEPT_NO = @ID";
            this.com.Parameters.AddWithValue("@ID", id);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            int result = await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }
    }
}
