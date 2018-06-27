using System;

namespace ConsultaAcademica.Core
{
    public enum Situacao
    {
        [EnumValueAttribute(false)]
        Inativo = 0,

        [EnumValueAttribute(true)]
        Ativo = 1,

        [EnumValueAttribute(2)]
        Ambos = 2,

        [EnumValueAttribute(3)]
        UsuarioInexistente
    }
}