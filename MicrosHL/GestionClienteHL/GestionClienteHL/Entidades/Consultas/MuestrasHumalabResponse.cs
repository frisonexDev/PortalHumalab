﻿namespace GestionClienteHL.Entidades.Consultas;

public class MuestrasHumalabResponse
{
	public List<MuestrasMesActual> mesActual = new List<MuestrasMesActual>();
	public List<MuestrasMesAnterior> mesAnterior = new List<MuestrasMesAnterior>();
}

public class MuestrasMesActual
{
	public int dia_semana { get; set; }
	public int total_muestras { get; set; }
}

public class MuestrasMesAnterior
{
	public int dia_semana { get; set; }
	public int total_muestras { get; set; }
}