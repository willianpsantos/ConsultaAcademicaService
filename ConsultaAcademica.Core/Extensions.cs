using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsultaAcademica.Core
{
    public static class Extensions
    {
        public static Tuple<string, string> SepararDisciplinaTurma(this string value)
        {
            if(!Regex.IsMatch(value, @"^.*\s+\({1}[\s\w\-]+\){1}\s?$"))
            {
                return null;
            }

            var regexBegin = @"^.*\s+\({1}";
            var regexEnd = @"\({1}[\s\w\-]+\){1}\s?$";
            var matchesBegin = Regex.Matches(value, regexBegin);
            var matchesEnd = Regex.Matches(value, regexEnd);

            var regexReplaceAnyPlace = @"\s?\({1}\s?";
            var regexReplaceBegin = @"^\(\s?";
            var regexReplaceEnd = @"\s?\)\s?$";
            var disciplina = "";
            var turma = "";

            foreach (Match item in matchesBegin)
            {
                disciplina = Regex.Replace(item.Value, regexReplaceAnyPlace, "");
                disciplina = disciplina.TrimEnd().TrimStart();                
            }

            foreach (Match item in matchesEnd)
            {
                turma = Regex.Replace(item.Value, regexReplaceBegin, "");
                turma = Regex.Replace(turma, regexReplaceEnd, "");                
            }

            return new Tuple<string, string>(disciplina, turma);
        }
        
        public static Tuple<string, string> SepararNomeSobrenome(this string name)
        {
            var pieces = name.Split(' ');
            var firstname = pieces[0];
            var lastname = "";

            for (var i = 1; i < pieces.Length; i++)
            {
                lastname += $" {pieces[i]}";
            };

            lastname = lastname.TrimStart().TrimEnd();
            return new Tuple<string, string>(firstname, lastname);
        }

        public static string FormatarMatricula(this string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return (value.Length < 5)
                     ? value.PadLeft(5, '0')
                     : value;
        }

        public static string DesformatarCpf(this string value)
        {
            return value.Replace(".", "").Replace("-", "");
        }

        public static string ValidarEmail(this Database database, string email, string matricula)
        {
            if(string.IsNullOrEmpty(email))
            {
                return $"{matricula}{Constantes.ATUALIZAR_EMAIL_DOMINIO}";
            }

            var regex = @"^[\w.-]+@[\w.-]+\.[a-zA-Z]{2,6}$";

            if (Regex.IsMatch(email, regex))
            {
                return email.Trim();
            }

            database.SetSgbd(Sgbd.MySql);
            database.SetCommandType(DatabaseCommandType.Text);
            database.ClearParameters();

            database.SetCommand("SELECT COUNT(*) FROM mdl_user WHERE email LIKE CONCAT('%', @email, '%')");
            database.AddParameterInput("@email", email);
            var count = database.ScalarValue<int>();

            if (count == 0)
            {
                return email.Trim();
            }

            return $"{matricula}{Constantes.ATUALIZAR_EMAIL_DOMINIO}";
        }

        public static long? GetMoodleUsuarioId(this Database database, string cpf)
        {
            database.SetSgbd(Sgbd.MySql);
            database.SetCommandType(DatabaseCommandType.Text);
            database.ClearParameters();

            database.SetCommand("SELECT id FROM mdl_user WHERE username = @cpf");
            database.AddParameterInput("@cpf", cpf);
            return database.ScalarValue<long?>();
        }

        public static long? GetMoodleDisciplinaId(this Database database, string disciplinaNome)
        {
            database.SetSgbd(Sgbd.MySql);
            database.SetCommandType(DatabaseCommandType.Text);
            database.ClearParameters();

            var values = disciplinaNome.SepararDisciplinaTurma();

            if(values == null)
            {
                return null;
            }

            database.SetCommand("SELECT DISTINCT id " + 
                                "  FROM mdl_course " + 
                                " WHERE rtrim(ltrim(fullname)) LIKE CONCAT(RTRIM(LTRIM(@disciplina)), '%', RTRIM(LTRIM(@turma)), '%') ");

            database.AddParameterInput("@disciplina", values.Item1);
            database.AddParameterInput("@turma", values.Item2);
            return database.ScalarValue<long?>();
        }

        public static bool UsuarioEstaVinculadoADisciplina(this Database database, int roleId, long usuarioId, string disciplinaNome)
        {
            database.SetSgbd(Sgbd.MySql);
            database.SetCommandType(DatabaseCommandType.Text);
            database.ClearParameters();

            database.SetCommand(" SELECT COUNT(*) " +
                                "   FROM mdl_role_assignments rs " +
                                "    INNER JOIN mdl_context e ON rs.contextid = e.id " +
                                "    INNER JOIN mdl_course  c ON c.id = e.instanceid " +
                                "  WHERE e.contextlevel = @contextlevel " +
                                "    AND rs.roleid      = @role " +
                                "    AND c.visible      = 1 " +
                                "    AND rs.userid      = @userId " +
                                "    AND c.fullname LIKE CONCAT(@descricao, '%') ");

            database.AddParameterInput("@userId", usuarioId);
            database.AddParameterInput("@descricao", disciplinaNome);
            database.AddParameterInput("@contextlevel", Constantes.CONTEXTLEVEL_CURSO);
            database.AddParameterInput("@role", roleId);
            var count = database.ScalarValue<int>();

            return count > 0;
        }

        public static void AtivarUsuarioMoodle(this Database database, long userId)
        {
            var suspended = 0;

            database.SetSgbd(Sgbd.MySql);
            database.SetCommandType(DatabaseCommandType.Text);
            database.ClearParameters();

            database.SetCommand("UPDATE mdl_user SET suspended = @suspended WHERE id = @id");
            database.AddParameterInput("@id", userId);
            database.AddParameterInput("@suspended", suspended);
            database.Execute(true);

            database.ClearParameters();
            database.SetCommand("INSERT INTO mdl_logstore_user_suspended (userid, suspended) VALUES (@userid, @suspended)");
            database.AddParameterInput("@userid", userId);
            database.AddParameterInput("@suspended", suspended);
            database.Execute(true);
        }

        public static void DesativarUsuarioMoodle(this Database database, long userId)
        {
            var suspended = 1;

            database.SetSgbd(Sgbd.MySql);
            database.SetCommandType(DatabaseCommandType.Text);
            database.ClearParameters();

            database.SetCommand("UPDATE mdl_user SET suspended = 1 WHERE id = @id");
            database.AddParameterInput("@id", userId);
            database.AddParameterInput("@suspended", suspended);
            database.Execute(true);

            database.ClearParameters();
            database.SetCommand("INSERT INTO mdl_logstore_user_suspended (userid, suspended) VALUES (@userid, @suspended)");
            database.AddParameterInput("@userid", userId);
            database.AddParameterInput("@suspended", suspended);
            database.Execute(true);
        }

        public static void AtivarUsuarioMoodle(this Database database, string cpf)
        {
            long? userId = GetMoodleUsuarioId(database, cpf);

            if(!userId.HasValue)
            {
                return;
            }

            AtivarUsuarioMoodle(database, userId.Value);
        }

        public static void DesativarUsuarioMoodle(this Database database, string cpf)
        {
            long? userId = GetMoodleUsuarioId(database, cpf);

            if (!userId.HasValue)
            {
                return;
            }

            DesativarUsuarioMoodle(database, userId.Value);
        }

        public static bool IsXml(this string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                return false;

            return text.StartsWith("<?xml version=\"1.0\"");
        }

        public static bool IsJson(this string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                return false;

            var pattern = @"(\[+\{+.*\}+\]+)|(\[\])|(\{\})|(\{+.*\}+)";
            return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsNullResponse(this string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                return false;

            return text == "null" || text == "NULL" || text == "Null";
        }

        public static bool IsMoodleError(this string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                return false;

            var pattern = "^\\{{1}\\s?\"?(exception|EXCEPTION|Exception)+\"?\\s?\\:{1}";
            return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
        }

        public static string Quote(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "NULL";
            }

            if (value.Contains("'"))
            {
                value = value.Replace("'", "\\'");
            }

            return $"'{value}'";
        }

        public static bool DateEquals(this DateTime date, DateTime otherDate)
        {
            return date.Day == otherDate.Day &&
                   date.Month == otherDate.Month &&
                   date.Year == otherDate.Year &&
                   date.Hour == otherDate.Hour &&
                   date.Minute == otherDate.Minute;
        }
    }
}
