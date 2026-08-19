# Reglas del Proyecto — LinkUp Pro

Este documento reúne, en forma de reglas de negocio y requerimientos funcionales, los criterios de evaluación del mini proyecto **LinkUp Pro**.

---

## 1. Funcionalidades generales

1. El menú principal debe incluir todas las opciones requeridas por la aplicación.
2. La navegación entre todos los módulos debe funcionar correctamente.
3. El layout general de la aplicación debe usarse de forma consistente en todas las pantallas.
4. Los mensajes de validación, confirmación y acceso denegado deben ser claros para el usuario.
5. Debe protegerse el acceso directo por URL y validarse siempre la sesión activa.

---

## 2. Login, registro y recuperación de cuenta

1. La pantalla de inicio de sesión debe incluir usuario, contraseña y la opción "Mantener sesión iniciada".
2. Si un usuario autenticado intenta acceder al login, debe ser redirigido al Home.
3. Debe existir un control para mostrar y ocultar la contraseña.
4. Las credenciales incorrectas deben mostrar un mensaje genérico (sin indicar si el usuario o la contraseña es lo incorrecto).
5. No se debe permitir el acceso a cuentas que aún no han sido activadas.
6. Tras cinco intentos fallidos, la cuenta debe bloquearse temporalmente durante 15 minutos.
7. "Mantener sesión iniciada" debe mantener la sesión activa por un máximo de siete días.
8. Si no se mantiene la sesión, esta debe cerrarse automáticamente tras 30 minutos de inactividad.
9. El registro debe solicitar todos los campos personales y credenciales requeridos.
10. El teléfono debe validarse con el formato de República Dominicana.
11. El nombre de usuario y el correo deben ser únicos, sin distinguir mayúsculas de minúsculas.
12. La foto de perfil debe validarse en formato permitido y tamaño máximo.
13. Debe mostrarse un indicador visual de fortaleza de contraseña, con su respectiva validación.
14. Toda cuenta nueva debe crearse en estado inactivo y debe enviarse un correo de activación.
15. La activación de cuenta debe hacerse mediante un token válido, vigente y de un solo uso.
16. El reenvío del correo de activación solo puede solicitarse con un mínimo de cinco minutos de espera entre envíos.
17. La recuperación de contraseña debe enviar el enlace correspondiente sin revelar si la cuenta existe o no.
18. Al restablecer la contraseña, debe desbloquearse la cuenta e invalidarse todas las sesiones anteriores.

---

## 3. Publicaciones (Home)

1. Las publicaciones propias deben listarse de la más reciente a la más antigua.
2. Cada publicación debe mostrar autor, fecha, contenido, multimedia, privacidad e interacciones de forma completa.
3. Debe existir un buscador de publicaciones propias por contenido.
4. Deben poder combinarse filtros por tipo, fechas y estado de edición.
5. Una publicación se crea con texto y la selección de una Imagen o un Video de YouTube.
6. Toda publicación debe tener obligatoriamente texto y exactamente un contenido multimedia.
7. Los campos "Imagen" y "Enlace de YouTube" deben comportarse de forma condicional entre sí.
8. Deben validarse el texto, el archivo de imagen y el enlace de YouTube.
9. Los videos de YouTube deben reproducirse de forma incrustada dentro de la aplicación.
10. Debe manejarse correctamente la privacidad "Solo amigos" y "Solo yo".
11. La opción "Permitir comentarios" debe estar activa por defecto.
12. Solo el autor de una publicación puede editarla.
13. Al cambiar entre imagen y video, no debe conservarse el contenido multimedia anterior.
14. Al cambiar la privacidad de una publicación, deben conservarse los comentarios, respuestas y reacciones.
15. Activar o desactivar los comentarios no debe eliminar las interacciones anteriores.
16. La eliminación de publicaciones debe ser lógica y deben retirarse de todos los listados.
17. Los comentarios deben poder crearse y visualizarse con sus respectivas validaciones.
18. Las respuestas deben poder crearse manteniendo conversaciones anidadas.
19. Solo el autor de un comentario o respuesta puede editarlo o eliminarlo.
20. Al eliminar un comentario, debe conservarse el hilo mostrando el mensaje "Este comentario fue eliminado".
21. Las reacciones "Me gusta" y "No me gusta" deben poder registrarse, cambiarse y eliminarse.

---

## 4. Notificaciones de interacción

1. Deben generarse notificaciones por comentarios, respuestas y reacciones de otros usuarios.
2. El destinatario de la notificación debe asignarse correctamente según el tipo de interacción.
3. Las notificaciones deben listarse de la más reciente a la más antigua.
4. El acceso al contenido relacionado debe respetar existencia, amistad y privacidad.
5. Deben manejarse los estados "Leída" y "No leída", junto con la opción "Marcar todas como leídas".
6. El contador de notificaciones no leídas debe actualizarse al cargar o recargar la pantalla.

---

## 5. Amigos

1. Debe mostrarse un resumen correcto de amigos activos y publicaciones disponibles.
2. Las publicaciones "Solo amigos" deben listarse de la más reciente a la más antigua.
3. Debe existir un buscador y filtros combinables para publicaciones de amigos.
4. Debe validarse la privacidad y la vigencia de la amistad antes de mostrar o permitir interacciones.
5. Debe permitirse comentar, responder y reaccionar en publicaciones de amigos.
6. Debe respetarse la configuración de comentarios desactivados.
7. Los videos de YouTube en publicaciones de amigos deben poder reproducirse.
8. El perfil del amigo debe mostrar sus datos y las publicaciones correspondientes visibles.
9. El listado de amigos debe ordenarse alfabéticamente, mostrando sus datos y acciones disponibles.
10. Debe existir un buscador de amigos por nombre, apellido o nombre de usuario.
11. Deben calcularse y listarse correctamente los amigos en común.
12. La relación de amistad debe manejar los estados "Activa" y "Eliminada".
13. La eliminación de una amistad debe ser lógica, bidireccional y requerir confirmación.
14. Al eliminar una amistad deben conservarse publicaciones, interacciones, notificaciones y partidas anteriores.
15. Las partidas existentes deben poder continuar, pero no deben poder iniciarse nuevas partidas tras eliminar la amistad.

---

## 6. Solicitudes de amistad

1. Debe mostrarse un contador de solicitudes recibidas pendientes de respuesta.
2. Deben manejarse los estados "En espera", "Aceptada", "Rechazada" y "Cancelada".
3. Las solicitudes pendientes deben listarse con toda la información requerida.
4. Deben calcularse y poder consultarse los amigos en común dentro de cada solicitud.
5. Aceptar una solicitud debe requerir confirmación y sus validaciones correspondientes.
6. Al aceptar una solicitud, la creación o reactivación de la amistad debe ser atómica.
7. Rechazar una solicitud debe requerir confirmación y conservar el historial.
8. Las solicitudes enviadas deben listarse con su estado y fecha de respuesta.
9. El emisor de una solicitud pendiente debe poder cancelarla.
10. Las solicitudes aceptadas o rechazadas deben poder eliminarse lógicamente del historial.
11. Debe listarse a los usuarios elegibles para una nueva solicitud de amistad.
12. Debe existir un buscador de usuarios disponibles por nombre de usuario.
13. Solo debe poder seleccionarse un único usuario mediante radio button.
14. Al enviar una solicitud, todas las validaciones deben ejecutarse en el servidor.
15. No deben permitirse solicitudes pendientes duplicadas en ninguna dirección.
16. Los registros de solicitudes y las relaciones de amistad deben mantenerse separados.
17. Debe existir autorización correcta para aceptar, rechazar, cancelar y ocultar solicitudes.

---

## 7. Battleship

1. Debe existir una pantalla principal con partidas activas e historial de partidas.
2. Las partidas activas deben listarse con oponente, fecha, duración y acciones disponibles.
3. Debe poder rendirse una partida activa, con confirmación y asignación correcta del ganador.
4. Debe listarse a los amigos activos disponibles para iniciar una partida.
5. Deben excluirse los amigos con quienes ya exista una partida activa.
6. Debe existir buscador, selección y validación del oponente.
7. La partida debe crearse correctamente y dar acceso a ambos jugadores.
8. Cada jugador debe colocar cinco barcos, con tamaños 2, 3, 3, 4 y 5.
9. Solo deben listarse los barcos que aún no han sido posicionados.
10. El tablero de posicionamiento debe ser de 12x12, con selección de celda inicial.
11. Debe poder seleccionarse la dirección: arriba, abajo, izquierda o derecha.
12. Debe validarse que ningún barco quede fuera del tablero.
13. Debe validarse que no exista superposición entre barcos.
14. Debe bloquearse la selección de celdas ya ocupadas.
15. Debe marcarse correctamente la finalización de la configuración de los cinco barcos.
16. Debe mostrarse una pantalla de espera mientras el otro jugador no ha finalizado su configuración.
17. La fase de ataque debe iniciarla el usuario que creó la partida.
18. El tablero de ataque debe mostrar los aciertos en rojo y los fallos en verde.
19. Los turnos deben alternarse estrictamente después de cada ataque.
20. Deben bloquearse los ataques fuera de turno o sobre celdas ya atacadas anteriormente.
21. Debe detectarse el hundimiento de barcos y finalizar la partida al hundirse toda la flota.
22. La actualización del tablero debe hacerse manualmente mediante "Refrescar pantalla".
23. Debe visualizarse el tablero propio sin revelar el tablero del oponente.
24. La partida debe finalizar por abandono si pasan 48 horas sin un ataque válido.
25. El historial debe incluir fechas, duración, resultado, ganador y resumen estadístico.
26. Debe poder consultarse el tablero final propio y del oponente, en modo de solo lectura.

---

## 8. Mi perfil

1. Deben visualizarse los datos personales, el correo y el nombre de usuario.
2. Al cargar el formulario, deben mostrarse los valores actuales y los campos de contraseña vacíos.
3. Deben poder editarse y validarse el nombre, apellido y teléfono.
4. La foto de perfil debe poder actualizarse de forma opcional y segura.
5. Si no se selecciona una nueva foto, debe conservarse la anterior.
6. El cambio de contraseña debe ser opcional y validar la contraseña actual.
7. Debe poder mostrarse u ocultarse la contraseña, junto con su indicador de fortaleza.
8. Tras cambiar la contraseña, deben invalidarse las sesiones y redirigir al login.
9. Los campos usuario, correo y estado no deben ser editables.
10. Los cambios en el perfil deben reflejarse de forma consistente en los demás módulos.
11. Cada usuario solo debe poder modificar su propio perfil.

---

## 9. Seguridad funcional

1. Las rutas internas deben protegerse con `[Authorize]` y las públicas con `[AllowAnonymous]`.
2. Debe autorizarse el acceso a cada recurso según propiedad, amistad, privacidad y participación.
3. Debe protegerse la aplicación contra manipulación de URL, identificadores y campos ocultos.
4. Debe implementarse protección CSRF en todas las operaciones que modifiquen información.
5. Deben usarse ViewModels para evitar asignación indebida de propiedades (over-posting).
6. Las contraseñas deben administrarse de forma segura mediante ASP.NET Core Identity.
7. Los tokens de activación y restablecimiento deben tener vigencia limitada y ser de un solo uso.
8. Debe evitarse revelar si un nombre de usuario o correo está registrado.
9. Las sesiones, cookies, HTTPS y el cierre de sesión deben manejarse de forma segura.
10. El contenido debe codificarse de forma segura para impedir HTML y JavaScript malicioso (XSS).
11. Deben validarse de forma segura las extensiones, el contenido, el tamaño y los nombres de los archivos.
12. Debe existir control de concurrencia para amistades, solicitudes, partidas, barcos y ataques.
13. Los errores deben manejarse sin revelar información técnica o sensible.
14. Todas las reglas sensibles deben validarse nuevamente en el servidor (no confiar solo en el cliente).

---

## 10. Reglas técnicas y de arquitectura

1. El proyecto debe implementarse en **ASP.NET Core MVC** con **.NET 9**.
2. Debe usarse **Entity Framework Core** con enfoque *Code First* y migraciones.
3. Debe implementarse correctamente la **arquitectura Onion**, respetando las responsabilidades de cada capa.
4. Deben usarse ViewModels con sus validaciones correspondientes.
5. Deben usarse DTOs para la transferencia de información entre capas.
6. Debe usarse **AutoMapper** entre entidades, DTOs y ViewModels.
7. Deben usarse repositorios genéricos para las operaciones comunes de acceso a datos.
8. Deben usarse servicios genéricos y servicios específicos para las reglas de negocio.
9. Los controladores no deben contener lógica de negocio compleja ni acceso directo al `DbContext`.
10. Las vistas no deben acceder directamente a los datos; deben basarse en ViewModels.
11. Debe usarse **ASP.NET Core Identity** para usuarios, contraseñas, tokens y sesiones.
12. Debe usarse una capa **Shared** para el envío de correo electrónico y el almacenamiento de imágenes.
13. La configuración externa, las credenciales y las cadenas de conexión deben protegerse adecuadamente.
14. Debe mantenerse consistencia entre operaciones relacionadas y sus validaciones en el servidor.
15. La interfaz debe ser clara, organizada y responsiva, usando Bootstrap u otro framework CSS.
16. El tablero de Battleship 12x12 debe ser visualmente comprensible.
17. Las operaciones de acceso a datos, correo y archivos deben ser asíncronas.
18. El manejo de fechas y errores debe ser consistente y sin revelar información sensible.
