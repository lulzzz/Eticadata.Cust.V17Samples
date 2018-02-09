﻿using System;
using System.Windows.Forms;
using Eticadata.ERP;
using Microsoft.Practices.CompositeUI;
using Eticadata.Common;
using Microsoft.Practices.ObjectBuilder;
using System.Data;
using Eticadata.Cust.DeskTop.Helpers;
using Eticadata.Infrastruct;
using System.Collections.Specialized;
using Eticadata.Cust.DeskTop.Models;
using System.Text;
using System.Collections.Generic;

namespace Eticadata.Cust.DeskTop.Views
{
    public partial class NewWindow : UserControl
    {
        public EtiAplicacao myEtiApp { get; set; }
        public WorkItem myWorkItem { get; set; }
        public UIUtils myUiutils { get; set; }


        /// <summary>
        /// Ao invocar a janela através da opção de menu na ribbon, irá ser injetado de forma automática estes objetos
        /// </summary>
        /// <param name="myWorkItem_"></param>
        /// <param name="myEtiApp_"></param>
        /// <param name="myUiutils_"></param>
        [InjectionConstructor()]
        public NewWindow([ServiceDependency()] WorkItem myWorkItem_,
           [ServiceDependency()] EtiAplicacao myEtiApp_,
           [ServiceDependency()] UIUtils myUiutils_)
        {
            InitializeComponent();

            myEtiApp = myEtiApp_;
            myUiutils = myUiutils_;
            myWorkItem = myWorkItem_;
        }

        private void NewWindow_Load(object sender, EventArgs e)
        {
            //Colocar os icons aos novos botões
            myEtiApp.Resources.Actualiza_IconWEB(btnExit, EtiIcons.Geral.Comandos.Cancelar);
            myEtiApp.Resources.Actualiza_IconWEB(btnSave, EtiIcons.Geral.Comandos.Gravar);
            myEtiApp.Resources.Actualiza_IconWEB(btnEntitiesCategory, EtiIcons.Geral.Comandos.Diagnostico);
        }


        /// <summary>
        /// Obtem uma tabela com as categorias da entidade
        /// Objetivo de ver como se usa a coneção á base de dados
        /// </summary>
        /// <returns></returns>
        private DataTable GetEntitiesCategory()
        {
            var conn = myEtiApp.ActiveEmpresa.GetConnectionBD();
            DataTable dt = null;

            try
            {
                var qwy = @"SELECT intCodigo    AS [Code],
                                   strDescricao AS [Description]
                            FROM   [Tbl_Categoria_Entidade] WITH (nolock) 
                            ";

                dt = conn.GetDataTable("Tbl_Categoria_Entidade", qwy);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        /// <summary>
        /// Obtem uma tabela com as categorias da entidade de uma base de dados especifica
        /// </summary>
        /// <returns></returns>
        private DataTable GetEntitiesCategoryOtherDatabase()
        {
            var conn = DatabaseCust.GetConnection(myEtiApp, @"DDS-VICTORG\SQL2014", "sa", "platinum", "emp_17676");
            DataTable dt = null;

            try
            {
                var qwy = @"SELECT intCodigo    AS [Code],
                                   strDescricao AS [Description]
                            FROM   [Tbl_Categoria_Entidade] WITH (nolock) 
                            ";

                dt = conn.GetDataTable("Tbl_Categoria_Entidade", qwy);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        /// <summary>
        /// Obtem uma tabela com as categorias da entidade, de um webservice embutido no ERP
        /// </summary>
        /// <returns></returns>
        private List<EntitiesCategory> GetEntitiesCategoryByWebservice()
        {
            var entitiesCategory = new List<EntitiesCategory>();

            //EtiWebClient já faz  a gestão dos cookies pois estamos dentro do ERP
            EtiWebClient web = new EtiWebClient();
            NameValueCollection param = new NameValueCollection();
            var serverUrl = myEtiApp.Ambiente.ServerUrl;

            try
            {
                var resultWS = web.DownloadData(serverUrl + "api/EntitiesCategoryTA/GetEntitiesCategory");
                entitiesCategory = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EntitiesCategory>>(System.Text.Encoding.UTF8.GetString(resultWS));
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return entitiesCategory;
        }

        private void btnEntitiesCategory_Click(object sender, EventArgs e)
        {
            //Da base de dados atual
            var res1 = GetEntitiesCategory();

            //De outra base de dados
            var res2 = GetEntitiesCategoryOtherDatabase();

            //Através de um webservice
            var res3 =  GetEntitiesCategoryByWebservice();
        }
    }
}
