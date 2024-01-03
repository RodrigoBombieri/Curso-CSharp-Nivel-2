using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominioP;
using negocioP;
using System.Configuration;

namespace pokemon1
{
    public partial class frmNuevoPokemon : Form
    {
        private Pokemon pokemon = null;

        private OpenFileDialog archivo = null;
        
        // esta funcion es llamada para agregar un nuevo pokemon.
        public frmNuevoPokemon()
        {
            InitializeComponent();
        }

        // esta funcion es llamada para modificar un pokemon
        public frmNuevoPokemon(Pokemon pokemon)
        {
            InitializeComponent();
            this.pokemon = pokemon;
            Text = "Modificar Pokemon";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {   
                // si el pokemon está en null, quiere decir que quiero agregar uno nuevo, si no esta null es porque quiero modificarlo.
                // por las funciones que estan aca arriba del todo. 
                if (pokemon == null)
                    pokemon = new Pokemon();

                pokemon.Numero = int.Parse(txtNumero.Text);
                pokemon.Nombre = txtNombre.Text;
                pokemon.Descripcion = txtDescripcion.Text;
                pokemon.UrlImagen = txtUrlImagen.Text;
                pokemon.Tipo = (Elemento)cboTipo.SelectedItem;
                pokemon.Debilidad = (Elemento)cboDebilidad.SelectedItem;

                if (pokemon.Id != 0)
                {
                    negocio.modificar(pokemon);
                    MessageBox.Show("Modificado exitosamente");
                }
                else
                {
                    negocio.agregar(pokemon);
                    MessageBox.Show("Agregado exitosamente");
                }

                // guardo imagen si la levanto localmente
                if(archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP")))
                {
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["poke-app"] + archivo.SafeFileName);
                }

                Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void frmNuevoPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementoNegocio = new ElementoNegocio();

            try
            {
                cboTipo.DataSource = elementoNegocio.listar();
                cboTipo.ValueMember = "Id";
                cboTipo.DisplayMember = "Descripcion";
                cboDebilidad.DataSource = elementoNegocio.listar();
                cboDebilidad.ValueMember = "Id";
                cboDebilidad.DisplayMember = "Descripcion";

                if (pokemon != null)
                {
                    txtNumero.Text = pokemon.Numero.ToString();
                    txtNombre.Text = pokemon.Nombre;
                    txtDescripcion.Text = pokemon.Descripcion;
                    txtUrlImagen.Text = pokemon.UrlImagen;
                    CargarImagen(pokemon.UrlImagen);
                    cboTipo.SelectedValue = pokemon.Tipo.Id;
                    cboDebilidad.SelectedValue = pokemon.Debilidad.Id;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            CargarImagen(txtUrlImagen.Text);
        }

        // hacemos una funcion para cargar la imagen, y si hay error muestra una imagen por defecto;
        private void CargarImagen(string imagen)
        {
            try
            {
                // el método load recibe un string URL, lo llamamos imagen.
                pcbPokemon.Load(imagen);
            }
            catch (Exception ex)
            {

                pcbPokemon.Load("https://i.pinimg.com/236x/97/20/1f/97201fd57103a823e6cadfc4be328c0f.jpg");
            }
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";
            // va a entrar si le doy OK cuando seleccione la imágen
            if(archivo.ShowDialog() == DialogResult.OK)
            {
                // el FileName guarda la ruta completa del archivo en la caja de texto
                txtUrlImagen.Text = archivo.FileName;
                CargarImagen(archivo.FileName);

                //para guardar la imágen en una carpeta de nuestro proyecto (cada vez que agreguemos o modifiquemos un poke, la va a guardar.. asique ojo)
                // Configuramos en el app config la key y el value.
                // Agregamos la referencia (click derecho sobre Referencias, Añadir Referencia) System.Configuration, y lo incluimos arriba de todo con using.System.Configuration; 
                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["poke-app"] + archivo.SafeFileName);
            }
        }
    }
} 
