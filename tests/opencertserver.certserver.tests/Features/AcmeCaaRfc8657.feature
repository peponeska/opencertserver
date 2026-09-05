@caa-rfc8657
Feature: RFC 8657 CAA account URI and validation method binding

    RFC 8657 defines the "accounturi" and "validationmethods" parameters on
    CAA "issue" and "issuewild" properties, refining which ACME account and
    which validation method a CA may use to authorize issuance.

    Rule: the accounturi parameter restricts issuance to a specific ACME account

        Scenario: a matching accounturi authorizes issuance
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "accounturi=https://acme.example.com/account/1234"
            And the ACME request uses the account "https://acme.example.com/account/1234"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must authorize issuance

        Scenario: a non-matching accounturi prevents authorization
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "accounturi=https://acme.example.com/account/0000"
            And the ACME request uses the account "https://acme.example.com/account/1234"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must reject issuance

        Scenario: an accounturi with an equivalent trailing slash is still a match
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "accounturi=https://acme.example.com/account/1234/"
            And the ACME request uses the account "https://acme.example.com/account/1234"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must authorize issuance

        Scenario: multiple accounturi parameters make the property unsatisfiable
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameters "accounturi=https://acme.example.com/account/1234;accounturi=https://acme.example.com/account/5678"
            And the ACME request uses the account "https://acme.example.com/account/1234"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must reject issuance

        Scenario: an invalid accounturi makes the property unsatisfiable
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "accounturi=not-a-uri"
            And the ACME request uses the account "https://acme.example.com/account/1234"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must reject issuance

        Scenario: an accounturi cannot be satisfied when the account URI is unknown
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "accounturi=https://acme.example.com/account/1234"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must reject issuance

        Scenario: a property without accounturi matches any account
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com"
            And the ACME request uses the account "https://acme.example.com/account/1234"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must authorize issuance

    Rule: the validationmethods parameter restricts the allowed validation methods

        Scenario: a listed validation method authorizes issuance
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "validationmethods=dns-01,http-01"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must authorize issuance

        Scenario: an unlisted validation method prevents authorization
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "validationmethods=dns-01"
            And the validation method "http-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must reject issuance

        Scenario: validation method labels are matched case-insensitively
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "validationmethods=DNS-01"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must authorize issuance

        Scenario: the BR alternative label for the website-ACME method authorizes http-01
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "validationmethods=ca-tbr-19"
            And the validation method "http-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must authorize issuance

        Scenario: the BR alternative label for the DNS-change method authorizes dns-01
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "validationmethods=ca-tbr-7"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must authorize issuance

        Scenario: the BR alternative label for another method prevents authorization
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "validationmethods=ca-tbr-7"
            And the validation method "http-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must reject issuance

        Scenario: a property without validationmethods allows any validation method
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com"
            And the validation method "tls-alpn-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must authorize issuance

    Rule: the accounturi and validationmethods parameters combine

        Scenario: both parameters must be satisfied together
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "accounturi=https://acme.example.com/account/1234; validationmethods=dns-01"
            And the ACME request uses the account "https://acme.example.com/account/1234"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must authorize issuance

        Scenario: a method restriction applies even when the account matches
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "accounturi=https://acme.example.com/account/1234; validationmethods=dns-01"
            And the ACME request uses the account "https://acme.example.com/account/1234"
            And the validation method "http-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must reject issuance

    Rule: any matching property in the RRset authorizes issuance

        Scenario: an authorizing property supersedes a restricting property
            Given a CAA "issue" record for "example.com" authorizing "ca.example.com" with the parameter "validationmethods=http-01"
            And a CAA "issue" record for "example.com" authorizing "ca.example.com"
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must authorize issuance

        Scenario: an empty issuer property does not authorize any CA
            Given a CAA "issue" record for "example.com" with an empty issuer
            And the validation method "dns-01"
            When CAA validation runs for "example.com"
            Then the CAA validation must reject issuance