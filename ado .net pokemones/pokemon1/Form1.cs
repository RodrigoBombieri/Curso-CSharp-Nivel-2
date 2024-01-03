using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominioP;
using negocioP;

namespace pokemon1
{
    public partial class Form1 : Form
    {
        // agrego esta lista para poder manipular la lista de pokemons
        private List<Pokemon> listaPokemon;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Número");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripción");
        }


        private void cargar()
        {
            PokemonNegocio negocio = new PokemonNegocio();
            try
            {
                // se recibe el datasource y se modela en el formulario, e invocamos el metodo listar que nos devuelve el listado que configuramos
                listaPokemon = negocio.listar();
                dgvPokemons.DataSource = listaPokemon;
                ocultarColumnas();
                // llama a la funcion para cargar la imagen de la posición 0 (el primer elemento y su URL).
                CargarImagen(listaPokemon[0].UrlImagen);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas()
        {
            dgvPokemons.Columns["UrlImagen"].Visible = false;
            dgvPokemons.Columns["Id"].Visible = false;
        }

        // ésta propiedad se activa cuando la selección actual cambia.
        private void dgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            // al actualizar la lista, la columna seleccionada queda vacia, entonces cuando vuelve a aparecer la lista completa, se llama a este metodo y selecciona la primer columna, y mientras no haya nada no falla
            if(dgvPokemons.CurrentRow != null)
            {
                // de atras para adelante, pedimos el objeto enlazado (URL de imagen .DataBoundItem), de la fila actual .CurrentRow (según la lista estamos en la 0), de la grilla de pokemons .dgvPokemon
                // guardo el contenido en una variable dentro del objeto Pokemon
                Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
                // cuando cambio la selección, se carga la otra imagen.
                CargarImagen(seleccionado.UrlImagen);

            }
            

        }

        // hacemos una funcion para cargar la imagen, y si hay error muestra una imagen por defecto;
        private void CargarImagen(string imagen)
        {
            try
            {
                // el método load recibe un string URL, lo llamamos imagen.
                pibPokemon.Load(imagen);
            }
            catch (Exception ex)
            {

                pibPokemon.Load("https://i.pinimg.com/236x/97/20/1f/97201fd57103a823e6cadfc4be328c0f.jpg");
            }
        }

        private void btnAgregarPokemon_Click(object sender, EventArgs e)
        {
            frmNuevoPokemon alta = new frmNuevoPokemon();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificarPokemon_Click(object sender, EventArgs e)
        {
            Pokemon seleccionado;
            seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
            
            
            frmNuevoPokemon modificar = new frmNuevoPokemon(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }

        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }

        private void eliminar(bool logico = false)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            Pokemon seleccionado;
            try
            {
                DialogResult respuesta = MessageBox.Show("¿De verdad querés eliminarlo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

                    if (logico)
                        negocio.eliminarLogico(seleccionado.Id);
                    else
                        negocio.eliminar(seleccionado.Id);

                    cargar();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            if(cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo para filtrar.");
                return true;
            }
            if(cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el criterio para filtrar.");
                return true;
            }
            if(cboCampo.SelectedItem.ToString() == "Número")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debes cargar el filtro para numérico.");
                    return true;

                }

                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Solo números para filtrar por un campo numérico.");
                    return true;
                }
            }

            return false;
        }

        private bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }

            return true;
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            PokemonNegocio negocio = new PokemonNegocio();
            
            try
            {
                if (validarFiltro())
                    return;
                
                // guardo el contenido de las box en variables
                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;

                dgvPokemons.DataSource = negocio.filtrar(campo, criterio, filtro);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            
        }

        private void txtFiltroRapido_TextChanged(object sender, EventArgs e)
        {
            List<Pokemon> listafiltrada;
            string filtro = txtFiltroRapido.Text;

            if (filtro != "")
            {
                listafiltrada = listaPokemon.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            }
            else
            {
                listafiltrada = listaPokemon;
            }

            dgvPokemons.DataSource = null;
            dgvPokemons.DataSource = listafiltrada;
            ocultarColumnas();
        }

        // si en el cboCampo selecciono como criterio de búsqueda un número, el criterio mostrará mayor a, menor a, igual a, y si selecciono
        // texto, muestra comienza con, termina con, contiene.
        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();

            if(opcion == "Número")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }
    }
}
