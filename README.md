(https://img.shields.io/github/license/shimat/opencvsharp.svg)](https://github.com/shimat/opencvsharp/blob/master/LICENSE) 

## Проект Test Console Application

- [Описание проекта](#about-project)

- [Запуск проекта](#run-project)
- [Соборка образа](#build-image)
- [Запуск контейнера](#run-container)
- [Терминал контейнера](#bash-containe)

## Использованные технологии
[![docker](https://img.shields.io/badge/-Docker-464646?style=flat-square&logo=docker)](https://www.docker.com/)
<p align="right">(<a href="#top">наверх</a>)</p>


### <a name="about-project">Проект Foodgram собирает рецепты пользователей на различные блюда.</a>

(![Project](https://github.com/MShelganov/TestConsoleApp))

<p align="right">(<a href="#top">наверх</a>)</p>

### <a name="run-project">Запуск проекта</a>
Чтобы развернуть проект на своей персональной машине необходимо:
Клонировать репозиторий:

```bash
git clone https://github.com/MShelganov/TestConsoleApp.git
```
Перейти в него в командной строке:

```bash
cd TestConsoleApp
```

<p align="right">(<a href="#top">наверх</a>)</p>

### <a name="build-image">Соборка образа</a>
Запустите терминал. Убедитесь, что вы находитесь в той же директории, где сохранён Dockerfile, и запустите сборку образа:
```
docker build -t testconsoleapp .
```

`build` — команда сборки образа по инструкциям из Dockerfile.
`-t testconsoleapp` — ключ, который позволяет задать имя образу, а потом и само имя.
` . ` — точка в конце команды — путь до Dockerfile, на основе которого производится сборка..

<p align="right">(<a href="#top">наверх</a>)</p>

### <a name="run-container">Запуск контейнера</a>
В терминале это делается командой:
```
docker run --name <имя контейнера> -it
```
`run` — команда запуска нового контейнера.
`--name my_project` — ключ, который позволяет задать имя контейнеру, и само имя.
`-it` — комбинация этих ключей даёт возможность передавать в контейнер команды из вашего терминала.

<p align="right">(<a href="#top">наверх</a>)</p>

### <a name="bash-container">Терминал контейнера</a>
Для входа в терминал контейнера выполните команду:
```
docker exec -it <CONTAINER ID> bash
```

`exec` — запустит команду внутри контейнера.
`-it` — комбинация ключей, которая передаёт команды из вашего терминала в контейнер.
`bash` — запустит терминал внутри контейнера.

Удалить контейнер:
```
docker container rm <CONTAINER ID>
```

Для получения <CONTAINER ID>:
```
docker container ls -a
```
