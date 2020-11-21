int f1(int a, int b, int c){
  if (a<b) c=2; else c=3;
}

void main(int x){
  int m1;
  int m2;
  int m3;
  f1(1,2,m3); /* Paso de parámetros correcto */
  f1(1,"2.2",m3); /*Debe indicar error de argumento 2 inválido*/
}