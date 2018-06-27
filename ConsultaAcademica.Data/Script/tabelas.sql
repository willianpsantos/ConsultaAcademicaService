create database importador_moodle;
use importador_moodle;

create table log_importacao
(
    dt_importacao 	 timestamp default current_timestamp,
    tp_importacao 	 varchar(100),    
    sucesso 		 bit default 1,
    sincronia	     bit default 0,
    suspenso		 bit default 0,
    url 			 varchar(800),
    dados_importados text,    
    dados_retornados text,
    mensagem         text,
    excecao          text
);


create table agendamento_execucao_servico
(
    dt_execucao varchar(30),
    hora_execucao varchar(5),
    executa_sincronia bit default 1,
    executa_completo bit default 0
);