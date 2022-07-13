(https://img.shields.io/github/license/shimat/opencvsharp.svg)](https://github.com/shimat/opencvsharp/blob/master/LICENSE) 

## ������ Test Console Application

- [�������� �������](#about-project)

- [������ �������](#run-project)
- [������� ������](#build-image)
- [������ ����������](#run-container)
- [�������� ����������](#bash-containe)

## �������������� ����������
[![docker](https://img.shields.io/badge/-Docker-464646?style=flat-square&logo=docker)](https://www.docker.com/)
<p align="right">(<a href="#top">������</a>)</p>


### <a name="about-project">������ Foodgram �������� ������� ������������� �� ��������� �����.</a>

(![Project](https://github.com/MShelganov/TestConsoleApp))

<p align="right">(<a href="#top">������</a>)</p>

### <a name="run-project">������ �������</a>
����� ���������� ������ �� ����� ������������ ������ ����������:
����������� �����������:

```bash
git clone https://github.com/MShelganov/TestConsoleApp.git
```
������� � ���� � ��������� ������:

```bash
cd TestConsoleApp
```

<p align="right">(<a href="#top">������</a>)</p>

### <a name="build-image">������� ������</a>
��������� ��������. ���������, ��� �� ���������� � ��� �� ����������, ��� ������� Dockerfile, � ��������� ������ ������:
```
docker build -t testconsoleapp .
```

`build` � ������� ������ ������ �� ����������� �� Dockerfile.
`-t testconsoleapp` � ����, ������� ��������� ������ ��� ������, � ����� � ���� ���.
` . ` � ����� � ����� ������� � ���� �� Dockerfile, �� ������ �������� ������������ ������..

<p align="right">(<a href="#top">������</a>)</p>

### <a name="run-container">������ ����������</a>
� ��������� ��� �������� ��������:
```
docker run --name <��� ����������> -it
```
`run` � ������� ������� ������ ����������.
`--name my_project` � ����, ������� ��������� ������ ��� ����������, � ���� ���.
`-it` � ���������� ���� ������ ��� ����������� ���������� � ��������� ������� �� ������ ���������.

<p align="right">(<a href="#top">������</a>)</p>

### <a name="bash-container">�������� ����������</a>
��� ����� � �������� ���������� ��������� �������:
```
docker exec -it <CONTAINER ID> bash
```

`exec` � �������� ������� ������ ����������.
`-it` � ���������� ������, ������� ������� ������� �� ������ ��������� � ���������.
`bash` � �������� �������� ������ ����������.

������� ���������:
```
docker container rm <CONTAINER ID>
```

��� ��������� <CONTAINER ID>:
```
docker container ls -a
```
