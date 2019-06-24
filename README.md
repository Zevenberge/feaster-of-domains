# Feaster of Domains

Feaster of Domains is a small proof-of-concept network of composable [microfrontends](https://martinfowler.com/articles/micro-frontends.html). It builds upon the technical idea of having small autonomous applications. Instead of having rigid boundaries between different frontends, the proof of concept utilises [design by introspection](https://www.youtube.com/watch?v=29h6jGtZD-U) to provide flexible composable interfaces without hard dependencies.

## History

Feaster of Domains is an insurance management application. Users log into their application, and register sold insurances, register claims and see the financial status. It is a minimised example of one of the projects at [Sogyo](https://www.sogyo.nl/). In fact, working on said project triggered me to think about this. We wanted to split our monolith into small microservices. We eventually figured that, even though we managed to cut our backend logic into pieces, we were keeping our monolithic UI. It is a large application with a lot of features that got added over the last 8 years. Upgrading the UI framework big bang to a more modern one is not feasible - just manually testing and verifying the sheer amount of features would take ages. Therefore we wanted to compose the UI from different microfrontends, allowing us to upgrade only the new parts of the application. Our users still expect to be able to navigate from screen to screen as if they work in a single application. They are right - why should our user experience degrade because we decided to play with a microservice architecture?

Meanwhile from a technical point of view, we want to keep our services as isolated as possible. We should still be able to manage clients seamlessly if our Finance server is down. From a design perspective, we don't want to hard-code dependencies on our services. Depending and acting on something that might or might not exist is an apparant paradox that left us puzzled for quite a while. However, being familiar with the D programming language, the paradigm of design by introspection was quite frequently used. In D, it is used to dynamically compose behaviour, depending on the capabilities of a given type. This proof of concept extends that behaviour to a service level.

## Repository structure

### FeasterOfDomains

The entry point for the end user is the Feaster of Domains web application. Its sole responsibility is offering an end point where users can connect to, log in, and move to the service of their choice. For this, it defines a dashboard. It provides a basic skeleton in which other applications can plug their views.

## Ethymology

As every project needs the coolest and most edgy name, the [Fantasy name generator](https://www.fantasynamegenerators.com/world-destroyer-names.php) was consulted. Out of the generated names, `Feaster of Domains` was the one that struck me as the most appropriate for a software project.