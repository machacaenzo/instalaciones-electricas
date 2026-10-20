/* 
   SERVICIO DE ALERTAS GLOBAL (Estilo Cyberpunk)
   Usa SweetAlert2 pero pre-configurado con nuestro diseño.
*/

// Configuración base (El "Tema" oscuro)
const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    background: 'rgba(20, 20, 20, 0.95)', // Fondo oscuro casi sólido
    color: '#ffffff',
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
});

// Función 1: Alerta de Éxito (Pop-up central)
function MostrarExito(mensaje) {
    Swal.fire({
        title: 'OPERACIÓN EXITOSA',
        text: mensaje,
        icon: 'success',
        background: '#0a0a0a', // Fondo negro
        color: '#fff',
        confirmButtonColor: '#D4FF00', // NEÓN AMARILLO
        confirmButtonText: '<span style="color:black; font-weight:800;">ACEPTAR</span>',
        backdrop: `rgba(0,0,0,0.8)`, // Fondo oscuro al abrir
        customClass: {
            popup: 'cyber-alert-popup' // Clase CSS que crearemos ahora
        }
    });
}

// Función 2: Alerta de Error (Pop-up central)
function MostrarError(mensaje) {
    Swal.fire({
        title: 'ERROR DEL SISTEMA',
        text: mensaje,
        icon: 'error',
        background: '#0a0a0a',
        color: '#fff',
        confirmButtonColor: '#333', // Botón gris para cerrar
        confirmButtonText: 'CERRAR',
        customClass: {
            popup: 'cyber-alert-popup border-danger'
        }
    });
}

// Función 3: Toast (Notificación chiquita arriba a la derecha)
function Notificar(mensaje, tipo = 'success') {
    Toast.fire({
        icon: tipo,
        title: mensaje
    });
}

// Función 4: Confirmación de Acción (Con Callback al Formulario)
function ConfirmarAccion(titulo, texto, idFormulario) {
    Swal.fire({
        title: titulo,
        text: texto,
        icon: 'warning',

        // Estilos Cyberpunk
        background: '#050505',
        color: '#fff',
        backdrop: `rgba(0,0,0,0.9)`,

        // Botones
        showCancelButton: true,
        confirmButtonColor: '#D4FF00', // Neón para el SÍ
        cancelButtonColor: 'transparent', // Transparente para el NO

        confirmButtonText: '<span style="color:black; font-weight:800;">CONFIRMAR</span>',
        cancelButtonText: '<span style="color:white; text-decoration:underline;">CANCELAR</span>',

        // Clases CSS personalizadas
        customClass: {
            popup: 'cyber-alert-popup glow',
            cancelButton: 'cyber-cancel-btn' // Vamos a crear esta clase en CSS
        }
    }).then((result) => {
        if (result.isConfirmed) {
            // SI DIJO QUE SÍ: Buscamos el formulario por ID y lo enviamos
            document.getElementById(idFormulario).submit();
        }
    });
}