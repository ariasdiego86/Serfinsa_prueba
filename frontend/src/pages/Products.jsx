import { useState, useEffect, useCallback, useMemo } from 'react'
import Navbar from '../components/Navbar'
import { productService } from '../services/api'
import { useAuth } from '../contexts/AuthContext'

// =========================================================
// Estado inicial del formulario de producto
// =========================================================
const EMPTY_FORM = {
  nombre: '',
  descripcion: '',
  precio: '',
  stock: '',
  tipoProducto: '',
}

/**
 * Página principal: Gestión de Productos (CRUD completo).
 *
 * Hooks utilizados y justificación:
 * - useState: múltiples piezas de estado independientes (lista de productos,
 *   formulario, modal, errores, loading). useState es ideal para estado local del componente.
 * - useEffect: carga inicial de productos cuando el componente se monta.
 *   Se ejecuta una sola vez (dependencia []) para no hacer peticiones repetidas.
 * - useCallback: memoriza fetchProducts y los handlers de formulario para evitar
 *   que se recreen en cada render y no causen re-renderizados innecesarios de hijos.
 * - useMemo: calcula estadísticas derivadas de la lista de productos solo cuando
 *   la lista cambia, evitando recálculos en cada render.
 */
export default function Products() {
  const { isAdmin } = useAuth()

  // Estado: lista de productos obtenida del backend
  const [products, setProducts] = useState([])
  // Estado: control de loading y errores globales
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')

  // Estado: control del modal (crear/editar)
  const [showModal, setShowModal] = useState(false)
  const [editingProduct, setEditingProduct] = useState(null) // null = creando, obj = editando

  // Estado: valores del formulario dentro del modal
  const [formData, setFormData] = useState(EMPTY_FORM)
  const [formErrors, setFormErrors] = useState({})
  const [formLoading, setFormLoading] = useState(false)

  // Estado: confirmación de eliminación
  const [deletingId, setDeletingId] = useState(null)

  // =========================================================
  // useEffect: carga inicial de productos
  // =========================================================
  // Se ejecuta una sola vez al montar el componente (dependencia vacía []).
  // Fetches products from the backend via axios (con el JWT inyectado por el interceptor).
  useEffect(() => {
    fetchProducts()
  }, [])

  // useCallback: memoriza fetchProducts para usarla en useEffect y como callback
  // sin causar re-ejecuciones del efecto en cada render.
  const fetchProducts = useCallback(async () => {
    setLoading(true)
    setError('')
    try {
      const response = await productService.getAll()
      setProducts(response.data)
    } catch (err) {
      setError('Error al cargar los productos. Verifica tu conexión.')
    } finally {
      setLoading(false)
    }
  }, [])

  // =========================================================
  // useMemo: estadísticas derivadas de la lista de productos
  // =========================================================
  // Se recalcula SOLO cuando cambia la lista de productos, no en cada render.
  // Esto es beneficioso si la lista es grande o el cálculo es costoso.
  const stats = useMemo(() => ({
    total: products.length,
    totalStock: products.reduce((sum, p) => sum + p.stock, 0),
    avgPrice: products.length > 0
      ? (products.reduce((sum, p) => sum + p.precio, 0) / products.length).toFixed(2)
      : '0.00',
  }), [products])

  // =========================================================
  // Validación del formulario de producto
  // =========================================================
  const validateForm = () => {
    const errors = {}
    if (!formData.nombre.trim()) {
      errors.nombre = 'El nombre es obligatorio.'
    }
    const precio = parseFloat(formData.precio)
    if (!formData.precio || isNaN(precio) || precio <= 0) {
      errors.precio = 'El precio debe ser mayor a 0.'
    }
    const stock = parseInt(formData.stock, 10)
    if (formData.stock === '' || isNaN(stock) || stock < 0) {
      errors.stock = 'El stock no puede ser negativo.'
    }
    return errors
  }

  // =========================================================
  // Handlers del formulario (useCallback para memoización)
  // =========================================================

  const handleOpenCreate = useCallback(() => {
    setEditingProduct(null)
    setFormData(EMPTY_FORM)
    setFormErrors({})
    setShowModal(true)
  }, [])

  const handleOpenEdit = useCallback((product) => {
    setEditingProduct(product)
    setFormData({
      nombre: product.nombre,
      descripcion: product.descripcion ?? '',
      precio: product.precio.toString(),
      stock: product.stock.toString(),
      tipoProducto: product.tipoProducto ?? '',
    })
    setFormErrors({})
    setShowModal(true)
  }, [])

  const handleCloseModal = useCallback(() => {
    setShowModal(false)
    setEditingProduct(null)
    setFormErrors({})
  }, [])

  // useCallback: memoriza handleInputChange para no recrearla en cada render.
  // Este handler se pasa como prop a los inputs del formulario.
  const handleInputChange = useCallback((e) => {
    const { name, value } = e.target
    setFormData(prev => ({ ...prev, [name]: value }))
    // Limpiar el error del campo cuando el usuario empieza a escribir
    setFormErrors(prev => ({ ...prev, [name]: '' }))
  }, [])

  const handleSubmit = useCallback(async (e) => {
    e.preventDefault()
    const errors = validateForm()
    if (Object.keys(errors).length > 0) {
      setFormErrors(errors)
      return
    }

    setFormLoading(true)
    try {
      const payload = {
        nombre: formData.nombre.trim(),
        descripcion: formData.descripcion.trim() || null,
        precio: parseFloat(formData.precio),
        stock: parseInt(formData.stock, 10),
        tipoProducto: formData.tipoProducto.trim() || null,
      }

      if (editingProduct) {
        // Modo edición: llamar a PUT /api/products/{id}
        await productService.update(editingProduct.id, payload)
        setSuccess('Producto actualizado correctamente.')
      } else {
        // Modo creación: llamar a POST /api/products
        await productService.create(payload)
        setSuccess('Producto creado correctamente.')
      }

      handleCloseModal()
      await fetchProducts() // Recargar la lista
    } catch (err) {
      const detail = err.response?.data?.detail || err.response?.data?.title
      setFormErrors({ general: detail || 'Error al guardar el producto.' })
    } finally {
      setFormLoading(false)
    }
  }, [formData, editingProduct, fetchProducts, handleCloseModal])

  const handleDelete = useCallback(async (id) => {
    setDeletingId(id)
    try {
      await productService.delete(id)
      setSuccess('Producto eliminado correctamente.')
      setProducts(prev => prev.filter(p => p.id !== id))
    } catch (err) {
      setError('Error al eliminar el producto.')
    } finally {
      setDeletingId(null)
    }
  }, [])

  // Limpiar mensajes de éxito automáticamente después de 3 segundos
  useEffect(() => {
    if (success) {
      const timer = setTimeout(() => setSuccess(''), 3000)
      return () => clearTimeout(timer)
    }
  }, [success])

  return (
    <>
      <Navbar />

      <div className="products-page">
        {/* Encabezado con estadísticas */}
        <div className="products-header">
          <h1>Gestión de Productos</h1>
          {/* Solo los Admin pueden crear productos */}
          {isAdmin && (
            <button className="btn btn-primary" onClick={handleOpenCreate}>
              + Nuevo Producto
            </button>
          )}
        </div>

        {/* Tarjetas de estadísticas (calculadas con useMemo) */}
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '1rem', marginBottom: '1.5rem' }}>
          <div className="card" style={{ textAlign: 'center' }}>
            <div style={{ fontSize: '2rem', fontWeight: '700', color: '#0f3460' }}>{stats.total}</div>
            <div style={{ color: '#718096', fontSize: '0.85rem' }}>Total productos</div>
          </div>
          <div className="card" style={{ textAlign: 'center' }}>
            <div style={{ fontSize: '2rem', fontWeight: '700', color: '#38a169' }}>{stats.totalStock}</div>
            <div style={{ color: '#718096', fontSize: '0.85rem' }}>Unidades en stock</div>
          </div>
          <div className="card" style={{ textAlign: 'center' }}>
            <div style={{ fontSize: '2rem', fontWeight: '700', color: '#d69e2e' }}>${stats.avgPrice}</div>
            <div style={{ color: '#718096', fontSize: '0.85rem' }}>Precio promedio</div>
          </div>
        </div>

        {/* Mensajes de feedback */}
        {success && <div className="alert alert-success">{success}</div>}
        {error && <div className="alert alert-error">{error}</div>}

        {/* Tabla de productos */}
        <div className="card">
          {loading ? (
            <div style={{ textAlign: 'center', padding: '3rem', color: '#718096' }}>
              Cargando productos...
            </div>
          ) : products.length === 0 ? (
            <div className="empty-state">
              <p style={{ marginBottom: '1rem' }}>No hay productos registrados.</p>
              {isAdmin && (
                <button className="btn btn-primary" onClick={handleOpenCreate}>
                  Crear el primer producto
                </button>
              )}
            </div>
          ) : (
            <div className="table-wrapper">
              <table className="table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Nombre</th>
                    <th>Descripción</th>
                    <th>Precio</th>
                    <th>Stock</th>
                    <th>Tipo</th>
                    {isAdmin && <th>Acciones</th>}
                  </tr>
                </thead>
                <tbody>
                  {products.map(product => (
                    <tr key={product.id}>
                      <td>{product.id}</td>
                      <td><strong>{product.nombre}</strong></td>
                      <td>{product.descripcion ?? '—'}</td>
                      <td>${product.precio.toFixed(2)}</td>
                      <td>
                        <span style={{ color: product.stock === 0 ? '#e53e3e' : 'inherit' }}>
                          {product.stock}
                        </span>
                      </td>
                      <td>{product.tipoProducto ?? '—'}</td>
                      {isAdmin && (
                        <td>
                          <div style={{ display: 'flex', gap: '0.5rem' }}>
                            <button
                              className="btn btn-warning"
                              style={{ padding: '0.3rem 0.8rem', fontSize: '0.8rem' }}
                              onClick={() => handleOpenEdit(product)}
                            >
                              Editar
                            </button>
                            <button
                              className="btn btn-danger"
                              style={{ padding: '0.3rem 0.8rem', fontSize: '0.8rem' }}
                              onClick={() => handleDelete(product.id)}
                              disabled={deletingId === product.id}
                            >
                              {deletingId === product.id ? 'Eliminando...' : 'Eliminar'}
                            </button>
                          </div>
                        </td>
                      )}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>

      {/* Modal de Crear/Editar Producto */}
      {showModal && (
        <div className="modal-overlay" onClick={handleCloseModal}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal__header">
              <h2 className="modal__title">
                {editingProduct ? 'Editar Producto' : 'Nuevo Producto'}
              </h2>
              <button className="modal__close" onClick={handleCloseModal} aria-label="Cerrar">
                ×
              </button>
            </div>

            {formErrors.general && (
              <div className="alert alert-error">{formErrors.general}</div>
            )}

            <form onSubmit={handleSubmit} noValidate>
              <div className="form-grid">
                {/* Nombre */}
                <div className="form-group full-width">
                  <label htmlFor="nombre">Nombre *</label>
                  <input
                    id="nombre"
                    name="nombre"
                    type="text"
                    className={`form-control ${formErrors.nombre ? 'is-invalid' : ''}`}
                    value={formData.nombre}
                    onChange={handleInputChange}
                    placeholder="Ej: Laptop HP 15"
                    disabled={formLoading}
                  />
                  {formErrors.nombre && <span className="error-text">{formErrors.nombre}</span>}
                </div>

                {/* Precio */}
                <div className="form-group">
                  <label htmlFor="precio">Precio *</label>
                  <input
                    id="precio"
                    name="precio"
                    type="number"
                    step="0.01"
                    min="0.01"
                    className={`form-control ${formErrors.precio ? 'is-invalid' : ''}`}
                    value={formData.precio}
                    onChange={handleInputChange}
                    placeholder="0.00"
                    disabled={formLoading}
                  />
                  {formErrors.precio && <span className="error-text">{formErrors.precio}</span>}
                </div>

                {/* Stock */}
                <div className="form-group">
                  <label htmlFor="stock">Stock *</label>
                  <input
                    id="stock"
                    name="stock"
                    type="number"
                    min="0"
                    className={`form-control ${formErrors.stock ? 'is-invalid' : ''}`}
                    value={formData.stock}
                    onChange={handleInputChange}
                    placeholder="0"
                    disabled={formLoading}
                  />
                  {formErrors.stock && <span className="error-text">{formErrors.stock}</span>}
                </div>

                {/* Tipo de Producto */}
                <div className="form-group full-width">
                  <label htmlFor="tipoProducto">Tipo de Producto</label>
                  <input
                    id="tipoProducto"
                    name="tipoProducto"
                    type="text"
                    className="form-control"
                    value={formData.tipoProducto}
                    onChange={handleInputChange}
                    placeholder="Ej: Electrónico, Ropa, Alimento"
                    disabled={formLoading}
                  />
                </div>

                {/* Descripción */}
                <div className="form-group full-width">
                  <label htmlFor="descripcion">Descripción</label>
                  <textarea
                    id="descripcion"
                    name="descripcion"
                    className="form-control"
                    value={formData.descripcion}
                    onChange={handleInputChange}
                    rows={3}
                    placeholder="Descripción opcional del producto..."
                    disabled={formLoading}
                    style={{ resize: 'vertical' }}
                  />
                </div>
              </div>

              <div className="modal__footer">
                <button
                  type="button"
                  className="btn btn-secondary"
                  onClick={handleCloseModal}
                  disabled={formLoading}
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  className="btn btn-primary"
                  disabled={formLoading}
                >
                  {formLoading ? (
                    <><span className="spinner" /> Guardando...</>
                  ) : (
                    editingProduct ? 'Actualizar' : 'Crear Producto'
                  )}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  )
}
