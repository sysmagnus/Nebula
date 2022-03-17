﻿namespace Nebula.Data.ViewModels
{
    /// <summary>
    /// items de productos.
    /// </summary>
    public class CpeDetail
    {
        /// <summary>
        /// Id del producto.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Unidad de Medida.
        /// </summary>
        public string CodUnidadMedida { get; set; }

        /// <summary>
        /// Código de producto SUNAT.
        /// </summary>
        public string CodProductoSunat { get; set; }

        /// <summary>
        /// Descripción del Item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Precio del Item.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Cantidad del Producto.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Tipo de operación IGV Sunat.
        /// </summary>
        public string IgvSunat { get; set; }

        /// <summary>
        /// Porcentaje IGV - 18.00.
        /// </summary>
        public decimal ValorIgv { get; set; }

        /// <summary>
        /// Monto IGV por Item.
        /// </summary>
        public decimal MtoIgvItem { get; set; }

        /// <summary>
        /// Base Imponible IGV por Item.
        /// </summary>
        public decimal MtoBaseIgvItem { get; set; }

        /// <summary>
        /// Precio Venta Item.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Tributo ICBPER.
        /// </summary>
        public bool TriIcbper { get; set; } = false;

        /// <summary>
        /// Valor Tributo ICBPER.
        /// </summary>
        public decimal ValorIcbper { get; set; }

        /// <summary>
        /// Monto Tributo ICBPER por Item.
        /// </summary>
        public decimal MtoTriIcbperItem { get; set; }
    }
}