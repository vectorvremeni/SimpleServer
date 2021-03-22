using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MVC
{
    /// <summary>
    /// тут мы будем хранить информацию о том, что хочет пользователь
    /// </summary>
    public class HTTPContext
    {
        /// <summary>
        /// он хочет чтобы сервер создал этот контроллер
        /// если пользователь ничего не передал, то вызываем контроллер поумолчанию - Home
        /// </summary>
        public String Controller { get; set; } = "Home";
        /// <summary>
        /// вызвал у него этот метод (если ничего не передали, то Index - таким образом, если мы хотим просто) зайти на сайт то нам не надо указывать ничего, просто название сайта
        /// </summary>
        public String Action { get; set; } = "Index";
        /// <summary>
        /// и передал ему эти параметры
        /// </summary>
        public String Params { get; set; }
    }
}
